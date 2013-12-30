@for %%X in (cl.exe) do @(
    @set FOUND=%%~$PATH:X
)

@if defined FOUND (
    @REM do nothing
) else (
    @if exist "C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\vcvarsall.bat" (
        call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\vcvarsall.bat"
    ) else (
        @if exist "C:\Program Files\Microsoft Visual Studio 12.0\VC\vcvarsall.bat" (
            call "C:\Program Files\Microsoft Visual Studio 12.0\VC\vcvarsall.bat"
        ) else (
            @echo "vcvarsall.bat not found in any known places"
            @exit /D 1
        )
    )
)


tools\nant\nant %*