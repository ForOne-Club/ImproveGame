@echo off
REM 获取当前脚本所在目录
set "CURRENT_DIR=%~dp0"

REM 获取ImproveGame的父目录
for %%I in ("%CURRENT_DIR%..") do set "PARENT_DIR=%%~fI"

REM 创建符号链接
echo 创建 SilkyUIAnalyzer 符号链接...
mklink /D "%PARENT_DIR%\SilkyUIAnalyzer" "%CURRENT_DIR%SilkyUIAnalyzer"

echo 创建 SilkyUIFramework 符号链接...
mklink /D "%PARENT_DIR%\SilkyUIFramework" "%CURRENT_DIR%SilkyUIFramework"

echo 符号链接创建完成
pause
