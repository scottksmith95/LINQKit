# LinqKit Release Notes

#### 1.1.x.x (28 October 2017)
* LinqKit.Core supports NET Standard 2.0
* LinqKit.EntityFrameworkCore depends on Microsoft.EntityFrameworkCore 2.0.0 when used in NET Standard 2.0 

#### 1.1.8.0 (27 October 2016)
* LinqKit is now reverted to use dependency on EntityFramework 6.x
* LinqKit.Core is a new package which does not have any dependency on EntityFramework or EntityFrameworkCore
* LinqKit.EntityFramework has been updated to a newer version, but no structural changes
* LinqKit.EntityFrameworkCore has been updated to a newer version, but no structural changes

#### 1.1.7.1 (28 July 2016)
* Added ExpressionStarter logic

#### 1.1.7 (29 June 2016)
* Added strong name signing for LinqKit.dll

#### 1.1.6 (28 June 2016)
* Removed portable target to fix issues with .Net Core 1.0 RTM when using this project

#### 1.1.5 (28 June 2016)
* .Net Core 1.0 RTM
* Changed dependency from JetBrains.Annotations to fix issues with System.Runtime

#### 1.1.4 (27 June 2016)
* Update to dotnet rc2removed dependency on EntityFramework