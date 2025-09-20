# ==========================
# /output_pipe.py (Python export)
# ==========================

"""
Module d'export des résultats côté Python.

Note: L'application WinForms fournit déjà un export CSV/PDF riche via
`FaceWebAppUI/OutputPipe.cs`. Ce module Python reste minimal pour les
usages scripts/CLI et les tests.
"""

from __future__ import annotations

import csv
from pathlib import Path
from typing import Iterable, Mapping, Any, Sequence


class OutputPipe:
    """Export minimal des résultats pour scripts Python."""

    def __init__(self, export_dir: Path | str = "exports") -> None:
        self.export_dir = Path(export_dir)
        self.export_dir.mkdir(parents=True, exist_ok=True)

    def export_to_csv(self, file_name: str, rows: Iterable[Mapping[str, Any]]) -> Path:
        """
        Exporte des résultats tabulaires vers un CSV UTF-8.

        Args:
            file_name: nom de fichier ou chemin relatif dans `export_dir`.
            rows: itérable de dicts homogènes (mêmes clés).

        Returns:
            Chemin complet du fichier écrit.
        """
        path = Path(file_name)
        if not path.is_absolute():
            path = self.export_dir / path
        rows = list(rows)
        if not rows:
            # Créer un CSV vide avec aucune ligne de données
            path.parent.mkdir(parents=True, exist_ok=True)
            with path.open("w", newline="", encoding="utf-8") as f:
                pass
            return path

        fieldnames: Sequence[str] = list(rows[0].keys())
        path.parent.mkdir(parents=True, exist_ok=True)
        with path.open("w", newline="", encoding="utf-8") as f:
            writer = csv.DictWriter(f, fieldnames=fieldnames)
            writer.writeheader()
            for r in rows:
                writer.writerow({k: r.get(k, "") for k in fieldnames})
        return path

    def export_to_pdf(self, file_name: str, title: str, summary: Mapping[str, Any] | None = None) -> Path:
        """
        Stub léger d'export PDF. Pour un rapport riche, utiliser l'export C#.
        Ici on génère un PDF minimaliste si ReportLab est installé, sinon un .txt.
        """
        try:
            from reportlab.lib.pagesizes import A4
            from reportlab.pdfgen import canvas

            path = Path(file_name)
            if not path.is_absolute():
                path = self.export_dir / path
            path.parent.mkdir(parents=True, exist_ok=True)

            c = canvas.Canvas(str(path), pagesize=A4)
            width, height = A4
            y = height - 72
            c.setFont("Helvetica-Bold", 16)
            c.drawString(72, y, title)
            y -= 24
            c.setFont("Helvetica", 10)
            if summary:
                for k, v in summary.items():
                    c.drawString(72, y, f"- {k}: {v}")
                    y -= 14
                    if y < 72:
                        c.showPage()
                        y = height - 72
            c.showPage()
            c.save()
            return path
        except Exception:
            # Fallback: écrire un .txt si reportlab non dispo
            path = Path(file_name)
            if not path.is_absolute():
                path = self.export_dir / (path.stem + ".txt")
            path.parent.mkdir(parents=True, exist_ok=True)
            with path.open("w", encoding="utf-8") as f:
                f.write(title + "\n")
                if summary:
                    for k, v in summary.items():
                        f.write(f"- {k}: {v}\n")
            return path
