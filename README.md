# AutoresApi
Descargar Sql-server

modificar archivo appsettings.Development.json, en el atributo defaultConnection escribir los datos de autentication del servidor, dejando igual el nombre de la base de datos.
una vez configurado aplicar el siguiente comando
  dotnet add-migration nombrequetuquieras
si sale exitoso aplicar el sigueinte comando
  dotnet update-database 

si todo sale correctamente aplicar el siguiente comando
  dotnet run
