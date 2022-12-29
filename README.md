# AutoresApi
Descargar Sql-server

modificar archivo appsettings.Development.json, en el atributo defaultConnection escribir los datos de autentication del servidor, dejando igual el nombre de la base de datos.

Aplicar los siguientes comandos 

  dotnet add package Microsoft.EntityFrameworkCore
  dotnet add package Microsoft.EntityFrameworkCore.SqlServer
  dotnet add package Microsoft.EntityFrameworkCore.Design   
  dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
Ejecutar dotnet tool install --global dotnet-ef

una vez echo aplicar el siguiente comando
  dotnet ef migrations add nombrequetuquieras
si sale exitoso aplicar el sigueinte comando
  dotnet ef database update

si todo sale correctamente aplicar el siguiente comando
  dotnet run
*******************************************************
