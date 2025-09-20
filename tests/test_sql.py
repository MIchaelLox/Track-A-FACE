import os
from pathlib import Path
import sqlite3

from sql import DatabaseInitializer, DatabaseManager


def test_initialize_database_creates_core_tables(tmp_path: Path, monkeypatch):
    # Rediriger la base vers un dossier temporaire
    test_db_path = tmp_path / "face_engine_test.db"

    # Monkeypatch DATABASE_CONFIG via DatabaseManager en passant un chemin explicite
    dbm = DatabaseManager(str(test_db_path))
    initializer = DatabaseInitializer()
    initializer.db_manager = dbm

    # Exécuter l'initialisation (ne doit pas lever)
    initializer.initialize_database()

    assert test_db_path.exists(), "La base SQLite devrait être créée"

    # Vérifier la présence des tables principales
    conn = sqlite3.connect(str(test_db_path))
    try:
        cur = conn.cursor()
        for table in ("inputs", "cost_factors", "calculation_results", "scenarios"):
            cur.execute("SELECT name FROM sqlite_master WHERE type='table' AND name=?", (table,))
            row = cur.fetchone()
            assert row and row[0] == table, f"Table manquante: {table}"
    finally:
        conn.close()
