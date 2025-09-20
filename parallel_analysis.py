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
from typing import Dict, Any, List, Tuple

from engine import CalculationEngine
from input_handler import InputHandler, ValidationError


def _run_one(engine: CalculationEngine, input_data: Dict[str, Any]) -> Tuple[Dict[str, Any], float]:
    """Exécute un scénario et retourne (résumé_json, total_cost).

    Lève ValidationError ou Exception si le scénario est invalide.
    """
    handler = InputHandler()
    inputs = handler.create_inputs_from_dict(input_data)
    summary = engine.calculate_restaurant_costs(inputs)
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
    scenarios: List[Dict[str, Any]], max_workers: int = 4
) -> Dict[str, Any]:
    """
    Exécute plusieurs scénarios en parallèle et retourne un résumé comparatif.

    Args:
        scenarios: liste de dictionnaires d'entrée (cf. Engine API)
        max_workers: nombre de threads (I/O-bound, SQLite/CPU léger)

    Returns:
        {
          "results": [ { session_name, total_cost, ... }, ... ],
          "best": { "index": int, "session_name": str, "total_cost": float }
        }
    """
    results: List[Dict[str, Any]] = []
    errors: List[Dict[str, Any]] = []

    engine = CalculationEngine()

    with ThreadPoolExecutor(max_workers=max_workers) as executor:
        future_map = {executor.submit(_run_one, engine, s): i for i, s in enumerate(scenarios)}
        for fut in as_completed(future_map):
            idx = future_map[fut]
            try:
                res, total = fut.result()
                res["index"] = idx
                results.append(res)
            except ValidationError as e:
                errors.append({"index": idx, "error": "validation_error", "message": str(e)})
            except Exception as e:
                errors.append({"index": idx, "error": "execution_error", "message": str(e)})

    # Choisir le meilleur (coût total minimal)
    best = None
    if results:
        best_item = min(results, key=lambda r: r["total_cost"])
        best = {"index": best_item["index"], "session_name": best_item["session_name"], "total_cost": best_item["total_cost"]}

    return {"results": sorted(results, key=lambda r: r["index"]), "best": best, "errors": errors}
