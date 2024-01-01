
## WebApi

### `POST /api/upload`

**Descripción:**
Sube un archivo de video al bucket. El nombre del archivo se procesa para eliminar espacios, convertir a minúsculas y evitar duplicados.

**Parámetros de entrada:**
- `file` (tipo: IFormFile): El archivo de video que se va a subir.

**Respuestas:**
- `200 OK`: El archivo de video se subió exitosamente.
- `400 Bad Request`: Archivo no válido o formato de archivo incorrecto.

**Notas adicionales:**
- Se admite un conjunto específico de extensiones de archivo de video: `.mp4`, `.avi`, `.mkv`, `.mov`, `.wmv`.
- Se garantiza que los nombres de archivo no tengan espacios y sean únicos para evitar duplicados en el bucket.

### `GET /api/getAllVideos`

**Descripción:**
Obtiene metadatos de todos los videos almacenados en el servicio de almacenamiento en la nube.

**Respuestas:**
- `200 OK`: La solicitud se procesó correctamente y se devuelven los metadatos de los videos.
- `Response body`:
```
[
  {
    "etiquetas": [
      "string",
      "string"
    ],
    "miniaturaUrl": "string",
    "nombre": "string",
    "videoUrl": "string"
  },
]
```
- `400 Bad Request`: Error al procesar la solicitud.

### `GET /api/search/{text}`

**Descripción:**
Busca videos por etiquetas. Se procesa el texto de búsqueda para eliminar caracteres no alfanuméricos y convertir a minúsculas.

**Parámetros de entrada:**
- `text` (tipo: string): El texto de búsqueda que contiene las etiquetas.

**Respuestas:**
- `200 OK`: La búsqueda se realizó correctamente y se devuelven los metadatos de los videos encontrados.
- `Response body`:
```
[
  {
    "etiquetas": [
      "string",
      "string"
    ],
    "miniaturaUrl": "string",
    "nombre": "string",
    "videoUrl": "string"
  },
]
```
- `400 Bad Request`: El texto de búsqueda está vacío.
- `400 Bad Request`: Error al procesar la solicitud.

**Notas adicionales:**
- Se eliminan caracteres no alfanuméricos del texto de búsqueda.
- Se convierte el texto de búsqueda a minúsculas.