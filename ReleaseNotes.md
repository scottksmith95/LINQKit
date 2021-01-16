# 1.1.22 (16 January 2021)
- [#127](https://github.com/scottksmith95/LINQKit/pull/127) - Added ExpandableAttribute for easy injecting expressions without Invoke. contributed by [sdanyliv](https://github.com/sdanyliv)
- [#124](https://github.com/scottksmith95/LINQKit/issues/124) - LINQKit does not support referencing expressions stored in properties (InvalidCastException) [bug]

# 1.1.21 (14 November 2020)
- [#133](https://github.com/scottksmith95/LINQKit/pull/133) - Add support for EntityFramework 5.0.0 contributed by [StefH](https://github.com/StefH)

# 1.1.20 (29 October 2020)
- [#131](https://github.com/scottksmith95/LINQKit/pull/131) - Fix bug in version 1.1.19 (#129) contributed by [StefH](https://github.com/StefH)
- [#132](https://github.com/scottksmith95/LINQKit/pull/132) - Add PatchVersion to projects contributed by [StefH](https://github.com/StefH)
- [#129](https://github.com/scottksmith95/LINQKit/issues/129) - Version 1.1.19 introduces a breaking change to ExpressionExpander.Visit [bug]

# 1.1.19 (21 October 2020)
- [#121](https://github.com/scottksmith95/LINQKit/pull/121) - EntityFramework Core 5 fix [bug] contributed by [StefH](https://github.com/StefH)
- [#126](https://github.com/scottksmith95/LINQKit/pull/126) - Improved handling target Lambda from Property, Method, Compile() method. contributed by [sdanyliv](https://github.com/sdanyliv)
- [#116](https://github.com/scottksmith95/LINQKit/issues/116) - LinqKit fails with Entity Framework Core 5 [bug]
- [#118](https://github.com/scottksmith95/LINQKit/issues/118) - PredicateBuilder failed on EF Core 5.x [bug]

# 1.1.18 (02 February 2020)
- [#115](https://github.com/scottksmith95/LINQKit/pull/115) - Restructure EFCore dependencies [feature] contributed by [StefH](https://github.com/StefH)
- [#95](https://github.com/scottksmith95/LINQKit/issues/95) - Support .NET Core 3 [bug]
- [#114](https://github.com/scottksmith95/LINQKit/issues/114) - Restructure dependencies for LinqKit.Microsoft.EntityFrameworkCore for better support EFCore3 [feature]

# 1.1.17 (26 November 2019)
- [#94](https://github.com/scottksmith95/LINQKit/pull/94) - Update README.md contributed by [fkorak](https://github.com/fkorak)
- [#103](https://github.com/scottksmith95/LINQKit/pull/103) - EntityFramework 6.3.0 and Microsoft.EntityFrameworkCore 3.0.0 [bug, feature] contributed by [StefH](https://github.com/StefH)
- [#109](https://github.com/scottksmith95/LINQKit/pull/109) - Create build_and_test.yml [feature] contributed by [StefH](https://github.com/StefH)
- [#110](https://github.com/scottksmith95/LINQKit/pull/110) - Bump Microsoft.NETCore.UniversalWindowsPlatform from 5.2.2 to 5.2.4 in /examples/UWPExampleApp [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#98](https://github.com/scottksmith95/LINQKit/issues/98) - Clearer license sated in repo? [feature]
- [#101](https://github.com/scottksmith95/LINQKit/issues/101) - Support for EF 6.3, targeting net45 and netstandard2.1 [feature]
- [#102](https://github.com/scottksmith95/LINQKit/issues/102) - EF6 and EF Core version controversy [feature]

# 1.1.16 (20 March 2019)
- [#77](https://github.com/scottksmith95/LINQKit/pull/77) - Fix for small typo in PredicateBuilder section contributed by [tiesont](https://github.com/tiesont)
- [#92](https://github.com/scottksmith95/LINQKit/pull/92) - Support Z.Entity.Framework.Classic contributed by [StefH](https://github.com/StefH)
- [#79](https://github.com/scottksmith95/LINQKit/issues/79) - Reference Linqkit.Core and LinqKit.Microsoft.EntityFrameworkCore  in 1 solution
- [#80](https://github.com/scottksmith95/LINQKit/issues/80) - Feature: Release for latest version EF Core 2? [feature]
- [#81](https://github.com/scottksmith95/LINQKit/issues/81) - Change namespace of LinqKit.Microsoft.EntityFrameworkCore
- [#90](https://github.com/scottksmith95/LINQKit/issues/90) - Can't get simplest possible example to add where clause to SQL
- [#91](https://github.com/scottksmith95/LINQKit/issues/91) - Entity Framework Classic - Support

# 1.1.15 (21 February 2018)
- [#76](https://github.com/scottksmith95/LINQKit/issues/76) - Regression: ExpandableQuery&lt;T&gt; is inaccessible due to its protection level

# 1.1.13.0 (01 February 2018)
- [#61](https://github.com/scottksmith95/LINQKit/issues/61) - ExpressionVisitor doesn't handle all expression Types [bug]
- [#72](https://github.com/scottksmith95/LINQKit/issues/72) - PredicateBuilder failing to build correct expression

# 1.1.12.0 (31 December 2017)
- [#32](https://github.com/scottksmith95/LINQKit/issues/32) - Is it possible to support PCL and will it work with AsyncTableQuery&lt;T&gt; for SQLite queries?
- [#47](https://github.com/scottksmith95/LINQKit/issues/47) - .NET Native Compiler error
- [#63](https://github.com/scottksmith95/LINQKit/issues/63) - Upgrade projects to VS2017 csproj format and add support for NetStandard 2.0 [feature]
- [#67](https://github.com/scottksmith95/LINQKit/issues/67) - License information [bug]
- [#69](https://github.com/scottksmith95/LINQKit/issues/69) - missing licensingUrl node from nuspec in .NET45

# 1.1.11.0 (28 October 2017)
- [#54](https://github.com/scottksmith95/LINQKit/pull/54) - Rebind base parameter in PredicateBuilder instead of using Invoke contributed by [EdwardBlair](https://github.com/EdwardBlair)
- [#64](https://github.com/scottksmith95/LINQKit/pull/64) - Fix Markdown in Readme.md contributed by [leotsarev](https://github.com/leotsarev)
- [#65](https://github.com/scottksmith95/LINQKit/pull/65) - remove unnecessary dot in README.md contributed by [YuriiPovkh](https://github.com/YuriiPovkh)
- [#66](https://github.com/scottksmith95/LINQKit/pull/66) - Added example for &quot;.And&quot; predicatebuilder to readme.md contributed by [rsuk](https://github.com/rsuk)
- [#34](https://github.com/scottksmith95/LINQKit/issues/34) - .NET Core? [feature]
- [#56](https://github.com/scottksmith95/LINQKit/issues/56) - Update LinqKit.Microsoft.EntityFrameworkCore to use Microsoft.EntityFrameworkCore to 1.1.0 [feature]
- [#57](https://github.com/scottksmith95/LINQKit/issues/57) - PCL Compatibility
- [#58](https://github.com/scottksmith95/LINQKit/issues/58) - When predicate is too long,it throws System.StackOverflowException
- [#60](https://github.com/scottksmith95/LINQKit/issues/60) - Support for .NET 4.6.2?

# 1.1.9.0 (17 November 2016)
- [#1](https://github.com/scottksmith95/LINQKit/pull/1) - .NET 4.5.1 and EntityFramework 6: Async-await -support contributed by [Thorium](https://github.com/Thorium)
- [#6](https://github.com/scottksmith95/LINQKit/pull/6) - Incorrect parameters when using PredicateBuilder extension methods with Invoke() contributed by [dolly22](https://github.com/dolly22)
- [#10](https://github.com/scottksmith95/LINQKit/pull/10) - AggregateBalanced to fix issue #9 contributed by [Thorium](https://github.com/Thorium)
- [#11](https://github.com/scottksmith95/LINQKit/pull/11) - Run async EF6 queries async contributed by [axelheer](https://github.com/axelheer)
- [#18](https://github.com/scottksmith95/LINQKit/pull/18) - Add support for a .NET 3.5 version of the library. contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#19](https://github.com/scottksmith95/LINQKit/pull/19) - Allow Include to be invoked after AsExpandable (when EF is used). contributed by [artelk](https://github.com/artelk)
- [#20](https://github.com/scottksmith95/LINQKit/pull/20) - Linking to the EF UserVoice request  contributed by [alexdresko](https://github.com/alexdresko)
- [#24](https://github.com/scottksmith95/LINQKit/pull/24) - - added additional Invoke() overloads to support up to 16 parameters &#8230; contributed by [ViRuSTriNiTy](https://github.com/ViRuSTriNiTy)
- [#25](https://github.com/scottksmith95/LINQKit/pull/25) - Fix for issue #22. Proper expanding of arguments in method calls. contributed by [AndreyYurashevich](https://github.com/AndreyYurashevich)
- [#27](https://github.com/scottksmith95/LINQKit/pull/27) - LeftJoin contributed by [TanielianVB](https://github.com/TanielianVB)
- [#29](https://github.com/scottksmith95/LINQKit/pull/29) - Add Invoke method with five parameters contributed by [osjimenez](https://github.com/osjimenez)
- [#33](https://github.com/scottksmith95/LINQKit/pull/33) - Add dotnet core rc2 support and split into multiple versions/flavors contributed by [StefH](https://github.com/StefH)
- [#42](https://github.com/scottksmith95/LINQKit/pull/42) - Implemented ExpressionStarter&lt;T&gt; to eliminate the default 1=0 or 1=1 stub expressions [feature] contributed by [rhyous](https://github.com/rhyous)
- [#45](https://github.com/scottksmith95/LINQKit/pull/45) - Merge Development into Master contributed by [StefH](https://github.com/StefH)
- [#52](https://github.com/scottksmith95/LINQKit/pull/52) - Refactor projects contributed by [StefH](https://github.com/StefH)
- [#53](https://github.com/scottksmith95/LINQKit/pull/53) - LinqKit.Utitilies -&gt; LinqKit.Utilities contributed by [EdwardBlair](https://github.com/EdwardBlair)
- [#2](https://github.com/scottksmith95/LINQKit/issues/2) - Expand incorrectly handles nested expressions with anonymous type parameters
- [#3](https://github.com/scottksmith95/LINQKit/issues/3) - ExpressionExpander.VisitMethodCall(..) throws Exception: Cannot cast MethodCallExpressionN to LambdaExpression
- [#4](https://github.com/scottksmith95/LINQKit/issues/4) - How to accept arguments inside predicate?
- [#5](https://github.com/scottksmith95/LINQKit/issues/5) - Why own implementation of ExpressionVisitor?
- [#7](https://github.com/scottksmith95/LINQKit/issues/7) - queries that use AsExpandable and .ToListAsync() do NOT run asynchronously
- [#8](https://github.com/scottksmith95/LINQKit/issues/8) - Refactor to segregate EntityFramework dependencies
- [#9](https://github.com/scottksmith95/LINQKit/issues/9) - Too much parenthesis leads to stack overflow.
- [#12](https://github.com/scottksmith95/LINQKit/issues/12) - using AsExpandable() cause AsNoTracking doesn't work.
- [#15](https://github.com/scottksmith95/LINQKit/issues/15) - Nuget issue on OS with case-sensitive filesystem
- [#16](https://github.com/scottksmith95/LINQKit/issues/16) - Nuget-version is not up-to-date
- [#17](https://github.com/scottksmith95/LINQKit/issues/17) - Xml docs for IntelliSense [feature]
- [#21](https://github.com/scottksmith95/LINQKit/issues/21) - AsExpandable() causes &quot;new transaction is not allowed because there are other threads running in the session..&quot;
- [#22](https://github.com/scottksmith95/LINQKit/issues/22) - Predicate.Invoke(Predicate.Invoke(..)) does not work
- [#23](https://github.com/scottksmith95/LINQKit/issues/23) - Unbounded variable in expanded predicate
- [#26](https://github.com/scottksmith95/LINQKit/issues/26) - Unnecessary include of entity framework in nuget package
- [#31](https://github.com/scottksmith95/LINQKit/issues/31) - Missing Not() method in Predicate Builder [feature]
- [#35](https://github.com/scottksmith95/LINQKit/issues/35) - Update project to the .NET Core RTM [feature]
- [#36](https://github.com/scottksmith95/LINQKit/issues/36) - System.Interactive.Async load failure
- [#37](https://github.com/scottksmith95/LINQKit/issues/37) - Nuget package Upgrade 1.1.3.1 to 1.1.4 failed to add reference
- [#38](https://github.com/scottksmith95/LINQKit/issues/38) - NuGet binaries not strong named any more [feature]
- [#39](https://github.com/scottksmith95/LINQKit/issues/39) - The source IQueryable doesn't implement IDbAsyncEnumerable
- [#40](https://github.com/scottksmith95/LINQKit/issues/40) - Sign the library (strong name) for use in projects signed [feature]
- [#41](https://github.com/scottksmith95/LINQKit/issues/41) - Unable to restore packages
- [#43](https://github.com/scottksmith95/LINQKit/issues/43) - Unable to build the project
- [#48](https://github.com/scottksmith95/LINQKit/issues/48) - Performance optimization [feature]
- [#49](https://github.com/scottksmith95/LINQKit/issues/49) - Running FirstOrDefaultAsync with AsExpandable gives the error
- [#50](https://github.com/scottksmith95/LINQKit/issues/50) - Using the Linq.Expression.Optimizer with LinqKit in an ASP.NET (MVC) Application

