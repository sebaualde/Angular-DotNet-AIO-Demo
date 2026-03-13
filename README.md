# Angular + ASP.NET Core — All In One

Proyecto de ejemplo que muestra cómo integrar **Angular 21** con **ASP.NET Core 9** en un único repositorio, donde el backend actúa como host de la SPA en producción y ambos corren de forma independiente en desarrollo.

---

## ¿Qué hace este proyecto?

- El **backend** (ASP.NET Core 9) expone una API REST y, en producción, sirve los archivos estáticos compilados de Angular desde `wwwroot/spa`.
- El **frontend** (Angular 21) consume la API usando una URL relativa (`/api/...`) y corre de forma independiente en desarrollo con `ng serve`.
- En **publicación**, MSBuild compila Angular automáticamente, comprime los assets con Brotli y los copia al backend — un solo `dotnet publish` genera el artefacto completo.

---

## Stack

| Capa | Tecnología |
|---|---|
| Frontend | Angular 21, TypeScript 5.9, RxJS 7.8 |
| Backend | ASP.NET Core 9, .NET 9 |
| Documentación API | Microsoft.AspNetCore.OpenApi |
| Compresión | Response Compression + Brotli CLI |
| Autenticación | ASP.NET Core Identity (preparado, no activo) |

---

## Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [Node.js](https://nodejs.org/) (recomendado: LTS)
- [npm 11+](https://www.npmjs.com/)
- [Angular CLI 21](https://angular.dev/tools/cli) (`npm install -g @angular/cli`)

---

## Correr en desarrollo

En desarrollo, **backend y frontend corren por separado**. El proxy de Angular redirige las llamadas a `/api` al backend automáticamente.

### 1. Backend

```bash
cd AngularDotnetAllInOne
dotnet run --launch-profile https
```

Queda escuchando en `https://localhost:7181`.

### 2. Frontend

```bash
cd AngularDotnetAllInOne/ClientApp
npm install
ng serve
```

Queda escuchando en `http://localhost:4200`.

> **?? Importante:** Abrí el navegador en **`http://localhost:4200`**, no en `https://localhost:7181`.  
> El archivo `proxy.conf.json` redirige automáticamente `/api/*` ? `https://localhost:7181`.

### Documentación de la API (Swagger/OpenAPI)

Con el backend corriendo, la spec OpenAPI está disponible en:

```
https://localhost:7181/openapi/v1.json
```

---

## Publicar (Producción)

Un solo comando compila Angular, comprime los assets con Brotli y genera el artefacto del backend:

```bash
cd AngularDotnetAllInOne
dotnet publish -c Release -o ./publish
```

Lo que hace MSBuild internamente:
1. `npm run build -- --configuration production` ? compila Angular
2. Comprime los assets con `brotli-cli` (`.js`, `.css`, `.html`, etc.)
3. Copia los archivos compilados a `wwwroot/spa/`
4. Publica el backend con los archivos estáticos incluidos

El resultado en `./publish` es un artefacto autónomo: el backend sirve la SPA y la API desde el mismo proceso.

---

## Estructura del proyecto

```
AngularDotnetAllInOne/
??? ClientApp/                  # Proyecto Angular
?   ??? src/
?   ?   ??? app/
?   ?   ?   ??? Models/         # Interfaces TypeScript
?   ?   ?   ??? Services/       # Servicios HTTP
?   ?   ??? environments/       # Variables de entorno (dev/prod)
?   ??? proxy.conf.json         # Proxy de desarrollo ? redirige /api al backend
?   ??? angular.json
??? Controllers/                # Controladores de la API
??? ProgramConfigs/             # Configuración modularizada del pipeline
?   ??? SecurityConfigs.cs      # Identity, cookies, CORS
?   ??? ServicesConfigs.cs      # Servicios, compresión, OpenAPI
?   ??? MiddlewaresConfigs.cs   # Pipeline de middlewares
??? wwwroot/
?   ??? spa/                    # Assets de Angular compilados (generados al publicar)
??? Program.cs
```

---

## Cómo funciona el proxy en desarrollo

```
Navegador ? http://localhost:4200/api/...
                        ?
                  proxy.conf.json
                        ?
                        ?
          https://localhost:7181/api/...  (ASP.NET Core)
```

En producción no hay proxy: el navegador habla directamente con el backend en el mismo origen, y las rutas `/api/*` las resuelve ASP.NET Core antes de que la SPA las capture.

---

## Notas

- **Identity** está preparado en `SecurityConfigs.cs` pero comentado. Para activarlo, descomentá la configuración de `AddIdentity` y agregá un `DbContext` que herede de `IdentityDbContext`.
- **CORS** está configurado para aceptar `http://localhost:4200` en desarrollo y `FrontendUrl` (desde `appsettings.json`) en producción.
- Los archivos de `ClientApp/` y `wwwroot/images/` están excluidos del publish para mantener el artefacto limpio.
