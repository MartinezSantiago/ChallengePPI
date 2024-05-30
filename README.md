Evidencia:https://drive.google.com/file/d/1ipruvU834DBkb07k1RYWPUbvtU1j_0Fr/view?usp=sharing

Sistema de Gestión de Órdenes de Inversión
Este proyecto es una aplicación de demostración diseñada para gestionar órdenes de inversión en el mercado financiero a través de una API RESTful. La aplicación permite a los usuarios realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) en órdenes de inversión, con funcionalidades como autenticación JWT, persistencia de datos, y seguridad implementadas.

Tecnologías Utilizadas
C# y ASP.NET Core: Para el desarrollo de la API REST.
SQL Server: Base de datos principal para almacenar órdenes de inversión.
Docker: Para la contenerización y despliegue del sistema.
JSON Web Tokens (JWT): Para autenticación segura de usuarios.
NUnit y MOQ: Para pruebas unitarias.
Características Principales
Operaciones CRUD: Los usuarios pueden crear, ver, actualizar y eliminar órdenes de inversión.
Autenticación JWT: Se implementa un sistema de autenticación seguro mediante tokens JWT.
Persistencia de Datos: Las órdenes de inversión se almacenan en una base de datos SQL Server.
Seguridad: Se aplican medidas de seguridad para proteger la API contra posibles amenazas.
Pruebas Unitarias: Se incluyen pruebas unitarias para validar la funcionalidad de la aplicación.
Arquitectura del Proyecto
El proyecto sigue una arquitectura basada en capas implementando principios de Dependency Injection (DI) y el patrón Repository para una mayor modularidad y mantenibilidad del código. Se ha utilizado el Strategy Pattern para manejar diferentes tipos de activos financieros y sus efectos en las órdenes de inversión.

Cómo Ejecutar el Proyecto
Requisitos Previos
.NET Core SDK
SQL Server
Docker Desktop (Opcional)

Ejecutar Migraciones de la Base de Datos:
Add-Migration, update-database, para generar el esquema en la base de datos.

PD: No llegué a probar el Docker Desktop =), se me rompió.
-Deje el appsettings.json con las credenciales. Obviamente una mala práctica, pero para que se vea como lo había implementado.
-Hice todo en inglés, sé que estaban en español los modelos, pero más que nada para hacerlo más ágil.
