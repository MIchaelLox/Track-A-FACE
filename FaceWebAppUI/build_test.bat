@echo off
echo === Test de compilation Track-A-FACE ===
echo.

echo Verification de .NET...
dotnet --version
if %errorlevel% neq 0 (
    echo ERREUR: .NET n'est pas installe ou accessible
    pause
    exit /b 1
)

echo.
echo Restauration des packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERREUR: Echec de la restauration des packages
    pause
    exit /b 1
)

echo.
echo Compilation du projet...
dotnet build --configuration Debug --verbosity detailed
if %errorlevel% neq 0 (
    echo ERREUR: Echec de la compilation
    pause
    exit /b 1
)

echo.
echo === COMPILATION REUSSIE ===
echo Le projet a ete compile avec succes.
echo.
echo Pour executer l'application:
echo dotnet run
echo.
pause
