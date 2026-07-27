# Guia de Mapeamento de Nulidade (SQL Server 2008 → C# .NET 10)

Para evitar erros de runtime como `SqlNullValueException: Data is Null`, siga este guia de conversão:

| Coluna SQL Server 2008 | Aceita NULL? | Tipo C# Moderno | Exemplo Fluent API (EF Core) |
|---|---|---|---|
| `VARCHAR` / `NVARCHAR` | SIM | `string?` | `.Property(x => x.Descricao).IsRequired(false);` |
| `INT` / `SMALLINT` | SIM | `int?` | `.Property(x => x.IdTipo).IsRequired(false);` |
| `DECIMAL` / `NUMERIC` | SIM | `decimal?` | `.Property(x => x.Valor).IsRequired(false);` |
| `DATETIME` | SIM | `DateTime?` | `.Property(x => x.DataAlteracao).IsRequired(false);` |
| `BIT` / `SMALLINT` | NÃO | `bool` | `.Property(x => x.Ativo).IsRequired();` |
