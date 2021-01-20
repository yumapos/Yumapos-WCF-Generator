1. For debug generation - run in DEBUG WCF-Generator, it run with default configuration (edit App.config)

2. For generate test repositories - run BUILD TestRepositoryGeneration project, it run WCF-Generator.exe with configuration file from solution items (edit WCF-Generator.exe.config)

При обновлении Visual studio до 16.8.2 перестал работать генератор, ошибка ".NET SDK not found". 
Установка различных версий сдк не принесла успеха. 
Пофиксилось обновлением в солюшене генератора пакета Microsoft.Build  до последней версии "16.8.0". 
Пока не заливаем релизную в ветку этот фикс т.к. не известно как себя ведет с предыдущей версией студии. (ветка hotfix/updateMsbuild)
