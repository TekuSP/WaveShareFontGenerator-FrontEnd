@echo off
go version >nul 2>&1 && (
    @echo on
    go version
    @echo off
) || (
    echo GO is missing on this PC, please install GO LANG
    pause
    exit /b
)
cd waveshareFontGenerator-CLI
go build -o ../GoLangLibraries/waveshareFontGenerator-CLI.exe -buildmode=exe
echo Done building ./GoLangLibraries/waveshareFontGenerator-CLI.exe
pause