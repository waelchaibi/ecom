@echo off
setlocal enabledelayedexpansion
cd /d "C:\Users\waelc\Desktop\dev\Shayma\Ecom\EcommerceAPI"

REM Create directories
if not exist "Models" mkdir Models
if not exist "DTOs" mkdir DTOs
if not exist "Services" mkdir Services
if not exist "Repositories" mkdir Repositories
if not exist "Data" mkdir Data

echo Directories created successfully
