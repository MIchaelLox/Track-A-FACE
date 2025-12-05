# ==========================
# /parallel_analysis.py
# ==========================
"""
Analyse parallèle et comparaison de scénarios pour Track-A-FACE.

Fournit des utilitaires pour exécuter plusieurs scénarios en parallèle et
comparer leurs coûts totaux, afin d'identifier la meilleure option.
"""
from __future__ import annotations

from concurrent.futures import ThreadPoolExecutor, as_completed
from typing import Any, Dict, List, Tuple, Optional

from engine import CalculationEngine
from input_handler import InputHandler, ValidationError


# ---------------------------------------------------------------------------
# Helpers fonctionnels pour l'API basée sur des dictionnaires
# ---------------------------------------------------------------------------

def _run_one_dict(
    input_data: Dict[str, Any],
    db_path: Optional[str] = None,
) -> Tuple[Dict[str, Any], float]:
    """
    Exécute un scénario à partir d'un dictionnaire d'entrées
    et retourne (résumé_json, total_cost).

    Utilisé par compare_scenarios(). Chaque appel utilise sa propre
    instance de CalculationEngine pour éviter de partager une connexion
    SQLite entre threads.
    """
    handler = InputHandler()
    inputs = handler.create_inputs_from_dict(input_data)

    engine = CalculationEngine(db_path) if db_path else CalculationEngine()
    try:
        summary = engine.calculate_restaurant_costs(inputs)
    finally:
        # S'assurer que la connexion SQLite n'est pas laissée ouverte
        if getattr(engine, "db_manager", None) and getattr(engine.db_manager, "connection", None):
            engine.db_manager.disconnect()

    return (
        {
            "session_id": summary.session_id,
            "session_name": summary.session_name,
            "staff_costs": summary.staff_costs,
            "equipment_costs": summary.equipment_costs,
            "location_costs": summary.location_costs,
            "operational_costs": summary.operational_costs,
            "total_cost": summary.total_cost,
        },
        summary.total_cost,
    )


def compare_scenarios(
    scenarios: List[Dict[str, Any]],
    max_workers: int = 4,
    db_path: Optional[str] = None,
) -> Dict[str, Any]:
    """
    Exécute plusieurs scénarios en parallèle (entrées sous forme de dict)
    et retourne un résumé comparatif.

    Args:
        scenarios: liste de dictionnaires d'entrée (cf. Engine API)
        max_workers: nombre de threads (I/O-bound, SQLite/CPU léger)
        db_path: chemin explicite vers la base SQLite (optionnel)

    Returns:
        {
          "results": [ { session_name, total_cost, ... }, ... ],
          "best": { "index": int, "session_name": str, "total_cost": float } | None,
          "errors": [ { "index": int, "error": str, "message": str }, ... ]
        }
    """
    results: List[Dict[str, Any]] = []
    errors: List[Dict[str, Any]] = []

    if not scenarios:
        return {"results": [], "best": None, "errors": []}

    with ThreadPoolExecutor(max_workers=max_workers) as executor:
        future_map = {
            executor.submit(_run_one_dict, s, db_path): i
            for i, s in enumerate(scenarios)
        }
        for fut in as_completed(future_map):
            idx = future_map[fut]
            try:
                res, total = fut.result()
                res["index"] = idx
                results.append(res)
            except ValidationError as e:
                errors.append(
                    {"index": idx, "error": "validation_error", "message": str(e)}
                )
            except Exception as e:
                errors.append(
                    {"index": idx, "error": "execution_error", "message": str(e)}
                )

    # Choisir le meilleur (coût total minimal)
    best = None
    if results:
        best_item = min(results, key=lambda r: r["total_cost"])
        best = {
            "index": best_item["index"],
            "session_name": best_item["session_name"],
            "total_cost": best_item["total_cost"],
        }

    return {
        "results": sorted(results, key=lambda r: r["index"]),
        "best": best,
        "errors": errors,
    }


# ---------------------------------------------------------------------------
# Classe ParallelAnalysis utilisée par les tests d'intégration avancés
# ---------------------------------------------------------------------------

class ParallelAnalysis:
    """
    Analyse multi-scénarios basée sur des RestaurantInputs.

    Utilisée notamment par les tests d'intégration avancés. Elle stocke
    un mapping nom_de_scénario -> RestaurantInputs, puis permet de :

      - calculer tous les scénarios (potentiellement en parallèle)
      - retourner un dict { nom: TotalCostSummary }
      - générer un rapport de comparaison lisible.

    La chaîne 'COMPARAISON' est toujours présente dans le rapport afin
    de faciliter les assertions des tests.
    """

    def __init__(self, engine: CalculationEngine, max_workers: int = 4) -> None:
        self.engine = engine
        self.max_workers = max_workers

        # Essayer de récupérer le chemin de la base de l'engine fourni
        self._db_path: Optional[str] = None
        try:
            self._db_path = str(engine.db_manager.db_path)
        except Exception:
            # Fallback: laisser CalculationEngine utiliser sa config par défaut
            self._db_path = None

        # nom -> RestaurantInputs
        self._scenarios: Dict[str, Any] = {}
        # nom -> TotalCostSummary
        self._results: Dict[str, Any] = {}
        # nom -> message d'erreur
        self._errors: Dict[str, str] = {}

    def add_scenario(self, name: str, inputs: Any) -> None:
        """Ajoute un scénario nommé à analyser plus tard."""
        self._scenarios[name] = inputs

    def _run_one_inputs(self, name: str, inputs: Any) -> Tuple[str, Any]:
        """
        Exécute un scénario (RestaurantInputs) avec une instance dédiée
        de CalculationEngine pour éviter de partager une connexion SQLite.
        """
        local_engine = CalculationEngine(self._db_path) if self._db_path else CalculationEngine()
        try:
            summary = local_engine.calculate_restaurant_costs(inputs)
            return name, summary
        finally:
            if getattr(local_engine, "db_manager", None) and getattr(local_engine.db_manager, "connection", None):
                local_engine.db_manager.disconnect()

    def calculate_all_scenarios(self) -> Dict[str, Any]:
        """
        Calcule tous les scénarios enregistrés.

        Returns:
            Dict[str, TotalCostSummary]: mapping nom_de_scénario -> résumé
        """
        self._results = {}
        self._errors = {}

        if not self._scenarios:
            return self._results

        with ThreadPoolExecutor(max_workers=self.max_workers) as executor:
            future_map = {
                executor.submit(self._run_one_inputs, name, inputs): name
                for name, inputs in self._scenarios.items()
            }

            for fut in as_completed(future_map):
                name = future_map[fut]
                try:
                    scenario_name, summary = fut.result()
                    self._results[scenario_name] = summary
                except ValidationError as e:
                    self._errors[name] = f"validation_error: {e}"
                except Exception as e:
                    self._errors[name] = f"execution_error: {e}"

        return self._results

    def generate_comparison_report(self) -> str:
        """
        Génère un rapport texte de comparaison des scénarios calculés.

        Le rapport contient :
          - 'COMPARAISON DES SCÉNARIOS'
          - le scénario optimal (coût total minimal)
          - la liste de tous les scénarios avec leur coût total
        """
        if not self._results:
            # Si rien n'a encore été calculé, tenter une fois
            if self._scenarios:
                self.calculate_all_scenarios()
            if not self._results:
                return "COMPARAISON DES SCÉNARIOS\nAucun scénario calculé."

        lines: List[str] = []
        lines.append("COMPARAISON DES SCÉNARIOS")
        lines.append("=" * 50)
        lines.append("")

        # Trier du moins cher au plus cher
        sorted_items = sorted(self._results.items(), key=lambda item: item[1].total_cost)
        best_name, best_summary = sorted_items[0]

        lines.append(f"Scénario optimal : {best_name}")
        lines.append(f"Coût total : {best_summary.total_cost:,.2f} CAD$")
        lines.append("")
        lines.append("Détails par scénario :")

        for name, summary in sorted_items:
            lines.append(f"- {name}: {summary.total_cost:,.2f} CAD$")

        if self._errors:
            lines.append("")
            lines.append("Scénarios en erreur :")
            for name, msg in self._errors.items():
                lines.append(f"- {name}: {msg}")

        return "\n".join(lines)
