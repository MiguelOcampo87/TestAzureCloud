# Proyecto: TestAzureCloud

Este proyecto es una prueba de integración con **Azure Blob Storage** en C#. Se encarga de subir imágenes a un contenedor en Azure, verificando si ya existen mediante un hash SHA256 para evitar duplicados.

## 📂 Contenido del Proyecto

- **Program.cs**: Contiene la lógica principal para cargar imágenes a Azure Blob Storage.
- **AppConfig.cs**: Administra la cadena de conexión de Azure Storage.

## 🚀 Funcionalidades

✅ Carga de imágenes a **Azure Blob Storage**  
✅ Verificación de existencia antes de la subida  
✅ Generación de **hash SHA256** para evitar duplicados  
✅ Creación automática del contenedor si no existe  
✅ Uso de metadatos para almacenar el hash del archivo  

## 🛠️ Configuración

1. **Configurar la conexión a Azure**  
   - Modificar la clase `AppConfig.cs` para incluir la cadena de conexión:
     ```csharp
     namespace TestAzureCloud
     {
         public class AppConfig
         {
             public static string AzureStorageConnectionString => "tu_cadena_de_conexion";
         }
     }
     ```

2. **Ejecutar la aplicación**  
   - Asegúrate de que los paquetes de **Azure.Storage.Blobs** están instalados:
     ```sh
     dotnet add package Azure.Storage.Blobs
     ```
   - Luego, ejecuta:
     ```sh
     dotnet run
     ```

## 📌 Código Principal (Program.cs)

```csharp
string connectionString = AppConfig.AzureStorageConnectionString;
await SubirImagen(connectionString, @"C:\Users\SYGNO\Pictures\1.jpg");
