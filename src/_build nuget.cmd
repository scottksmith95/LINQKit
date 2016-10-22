dotnet restore
dotnet pack -c Release LinqKit.Core\project.json
dotnet pack -c Release LinqKit\project.json
dotnet pack -c Release LinqKit.EntityFramework\project.json
dotnet pack -c Release LinqKit.Microsoft.EntityFrameworkCore\project.json
pause