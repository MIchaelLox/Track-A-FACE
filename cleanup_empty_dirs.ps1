# Script de nettoyage des dossiers vides - Track-A-FACE
# Exécuter depuis la racine du projet

Write-Host "🧹 NETTOYAGE DES DOSSIERS VIDES - Track-A-FACE" -ForegroundColor Green
Write-Host ""

$projectRoot = "TrackAFaceUI\TrackAFaceWinForms\TrackAFaceWinForms"

# Liste des dossiers vides à supprimer
$emptyDirs = @(
    "$projectRoot\Controls",
    "$projectRoot\Testing", 
    "$projectRoot\Workflow",
    "$projectRoot\TrackAFaceWinForms"
)

Write-Host "Dossiers vides identifiés:" -ForegroundColor Yellow
foreach ($dir in $emptyDirs) {
    if (Test-Path $dir) {
        $itemCount = (Get-ChildItem -Path $dir -Recurse -Force | Measure-Object).Count
        if ($itemCount -eq 0) {
            Write-Host "  ❌ $dir (vide)" -ForegroundColor Red
        } else {
            Write-Host "  ⚠️  $dir ($itemCount fichiers - NON VIDE!)" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ✓ $dir (déjà supprimé)" -ForegroundColor Gray
    }
}

Write-Host ""
$confirm = Read-Host "Voulez-vous supprimer les dossiers vides? (o/N)"

if ($confirm -eq 'o' -or $confirm -eq 'O') {
    foreach ($dir in $emptyDirs) {
        if (Test-Path $dir) {
            $itemCount = (Get-ChildItem -Path $dir -Recurse -Force | Measure-Object).Count
            if ($itemCount -eq 0) {
                try {
                    Remove-Item -Path $dir -Recurse -Force
                    Write-Host "  ✅ Supprimé: $dir" -ForegroundColor Green
                } catch {
                    Write-Host "  ❌ Erreur: $dir - $_" -ForegroundColor Red
                }
            } else {
                Write-Host "  ⏭️  Ignoré (non vide): $dir" -ForegroundColor Yellow
            }
        }
    }
    
    Write-Host ""
    Write-Host "✅ Nettoyage terminé!" -ForegroundColor Green
} else {
    Write-Host "❌ Nettoyage annulé." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "💡 Conseil: Ces dossiers ne sont pas trackés par Git." -ForegroundColor Cyan
Write-Host "   Ils ne seront pas inclus dans vos commits de toute façon." -ForegroundColor Cyan
