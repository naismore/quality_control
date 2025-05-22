#!/bin/bash

# 1. Запускаем тесты с сбором покрытия
echo "Запуск тестов с сбором покрытия..."
dotnet test --collect:"XPlat Code Coverage"

# 2. Находим последний созданный файл coverage.cobertura.xml
COVERAGE_FILE=$(find TestResults -name 'coverage.cobertura.xml' | head -1)

if [ -z "$COVERAGE_FILE" ]; then
    echo "Ошибка: Файл coverage.cobertura.xml не найден в папке TestResults"
    exit 1
fi

echo "Найден файл с покрытием: $COVERAGE_FILE"

# 3. Создаем папку для отчетов (если не существует)
mkdir -p coveragereport

# 4. Генерируем отчет
echo "Генерация отчета..."
reportgenerator -reports:"$COVERAGE_FILE" -targetdir:coveragereport -reporttypes:Html

# 5. Открываем отчет в браузере (для macOS)
if [[ "$OSTYPE" == "darwin"* ]]; then
    open coveragereport/index.html
else
    echo "Отчет сгенерирован: coveragereport/index.html"
    echo "Откройте его в браузере"
fi