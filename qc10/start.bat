@echo off

java -jar selenium-server-4.32.0.jar hub

java -Dwebdriver.chrome.driver=drivers\chromedriver.exe -Dwebdriver.gecko.driver=drivers\geckodriver.exe -jar selenium-server-4.32.0.jar node --hub http://localhost:4444
