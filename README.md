# SGG LevelDesign Tools

Пока что доступен один инструмент для level-дизайнеров, позволяющий мгновенно размещать выбранные объекты на поверхностях с коллайдерами. Поддерживает сложные иерархии объектов, различные режимы позиционирования и полностью интегрирован в Unity Editor.

## Установка
Добавьте в `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.sungeargames.ldtools_editor": "https://github.com/romanilyin/SGG_LDTools.git"
  }
}
```

Или через **Package Manager: Window > Package Manager > + > Add package from git URL...** 

Введите: https://github.com/romanilyin/SGG_LDTools.git

## Требования
- Unity 2021.3+
- .NET 4.x

## DropObjectsTool

### Что умеет
- **Точное позиционирование** на любых коллайдерах
- **Автоматическая обработка** сложных иерархий объектов
- **Три режима позиционирования**:
  - `Bottom`: По нижней грани bounding box
  - `Center`: По центру bounding box
  - `Pivot`: По опорной точке объекта
- **Пакетная обработка** множества объектов
- **Полная поддержка** Ctrl+Z
- **Логирование** проблемных объектов
- **Оптимизированная система** raycast'ов

### Использование
1. Выберите объекты в Unity Editor
2. Найдите новую кнопку со стрелкой вниз на панели инструментов
3. Нажмите кнопку или используйте горячую клавишу `D`.
4. Выберите режим позиционирования: Bottom, Center, Pivot 
5. Нажмите "Drop Objects"

### 🔧 Пример использования в коде
```csharp
// Ручной вызов функционала
DropObjectsTool.DropObjects(DropObjectsWindow.DropMode.Bottom);

// Расчет bounding box для сложного объекта
Bounds bounds = CalculateWorldBounds(gameObject);
Debug.Log($"Object size: {bounds.size}");
```
## Лицензия
```text
MIT License

Copyright (c) 2025 Roman Ilyin / SunGear Games

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## Поддержка
[Сообщить о проблеме](https://github.com/romanilyin/SGG_LDTools/issues) 

---
[romanilyin@github](https://https://github.com/romanilyin/) 

Inspired by [SnapToTerrain Tool by VladislavKovalenko](https://github.com/VladislavKovalenko/Snap-To-Terrain/tree/main?tab=readme-ov-file)