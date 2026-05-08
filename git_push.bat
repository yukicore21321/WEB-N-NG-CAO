@echo off
set /p msg="Nhap mo ta commit (Commit Message): "
if "%msg%"=="" (
    echo Loi: Ban phai nhap mo ta commit!
    pause
    exit /b
)

echo.
echo --- Dang chuan bi push code ---
git add .
git commit -m "%msg%"
git push
echo.
echo --- Hoan tat! ---
pause
