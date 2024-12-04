# Pangu.cs

[![](https://img.shields.io/nuget/v/PanguSpacing?logo=nuget&label=NuGet&color=%23004880)](https://www.nuget.org/packages/PanguSpacing)
[![](https://img.shields.io/nuget/vpre/PanguSpacing?logo=github&label=GitHub&color=%23181717)](https://github.com/otomad/pangu.cs)

Paranoid text spacing for good readability, to automatically insert whitespace between CJK (Chinese, Japanese, Korean), half-width English, digit and symbol characters.

## Usage

```csharp
string newText = Pangu.Spacing("當你凝視著bug，bug也凝視著你");
Console.WriteLine(newText); // "當你凝視著 bug，bug 也凝視著你"
```

## Licence

pangu.cs is available under the [MIT License][mitLink]. See the LICENSE file for more info.

[mitLink]:http://opensource.org/licenses/MIT

## References
- [pangu.js](https://github.com/vinta/pangu.js) (JavaScript)
- [pangu.cs](https://github.com/Roger-WIN/pangu.cs) (C#, archived)
- [pangu.go](https://github.com/vinta/pangu) (Go)
- [pangu.java](https://github.com/vinta/pangu.java) (Java)
- [pangu.py](https://github.com/vinta/pangu.py) (Python)
- [pangu.dart](https://github.com/SemonCat/pangu.dart)(Dart)
- [pangu.space](https://github.com/vinta/pangu.space) (Web API)
- [pangu.clj](https://github.com/coldnew/pangu.clj) (Clojure)
- [pangu.dart](https://github.com/SemonCat/pangu.dart) (Dart)
- [pangu.ex](https://github.com/cataska/pangu.ex) (Elixir)
- [pangu.objective-c](https://github.com/Cee/pangu.objective-c) (Objective-C)
- [pangu.php](https://github.com/Kunr/pangu.php) (PHP)
- [pangu.rb](https://github.com/dlackty/pangu.rb) (Ruby)
- [pangu.rs](https://github.com/airt/pangu.rs) (Rust)
- [pangu.swift](https://github.com/X140Yu/pangu.Swift) (Swift)
