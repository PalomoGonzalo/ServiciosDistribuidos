Este proyecto se centra en varias aplicaciones que todos se comunican entre si,

UserManager que es el servicio que se encarga el tema de logueo y altas de usuarios, perfiles.
Actualmente se encuentra online a traves de un servidor vps con docker y mysql como base de datos http://microserviciosmat.online:5025/swagger/index.html.

Control De Stock es una api que se encarga de crud del productos, la base de datos esta con sql server dentro del servidor vps con docker y la api con docker.
Cuenta con logs usando serilog que el path esta configurado en el appsettings, se obtiene informacion de jwt usando los claims, paginacion con storeprocedura y endpoint para descargar los logs. Para poder utilizar los endpoints es necesario obtener el token en el user manager. http://microserviciosmat.online:5024/swagger/index.html


Para poder utilizar los endpoints se necesita tener el jwt que se obtiene al loguear, string string (usuario y contraseña respectivamente)

![image](https://user-images.githubusercontent.com/89616271/225304602-d3a4fdf7-8942-4dcf-8a37-8db31b65c3dc.png)

Te devuelve el token

![image](https://user-images.githubusercontent.com/89616271/225304902-86c00350-8ced-481f-ab6a-eb82da5f9437.png)

Y en authorize escribir Bearer + token 

![image](https://user-images.githubusercontent.com/89616271/225305091-fd5d73ab-311b-4441-a8e0-4aa9c3c12142.png)

Los endpoints son cruds, con baja logica, contaseñas hassheadas, cada accion que se realiza se guarda eventos, como cambiar contraseña, registrar usuario, eliminar usuario obteniendo la ip de quien lo realizo
El endpoint de eventos, al buscar por id se obtiene los eventos.

1 = los eventos de registrar usuarios
2 = los eventos de baja de usuarios
3 = cambiar contraseña de usuario 

![image](https://user-images.githubusercontent.com/89616271/236542959-373860f5-d7e4-4b54-8776-7f6b9703ccb1.png)

![image](https://user-images.githubusercontent.com/89616271/236547028-cabd73dc-7dbf-4e41-90ca-7444fff128c2.png)



En control de stock de productos, se tiene que que poner en el authorize el jwt que se obtenio del user manager.

![image](https://user-images.githubusercontent.com/89616271/236544320-058e3081-fd27-4f33-babe-8d11ebf534cd.png)

![image](https://user-images.githubusercontent.com/89616271/236544579-3b48f3b9-4072-4645-81cb-b2b234bc2f90.png)

![image](https://user-images.githubusercontent.com/89616271/236544678-6ef29480-f7bd-4c6f-ba8b-113ee391b873.png)

Para descargar los logs tienen que tener el siguiente formato, log_productosxxxx.txt (donde xxxx es la fecha en formato yyyymmdd)

![image](https://user-images.githubusercontent.com/89616271/236545110-94af5e74-042e-4909-8469-ba9b1aad5e72.png)


![image](https://user-images.githubusercontent.com/89616271/236545369-2b0714be-4520-400b-9ece-e5c92ac05c07.png)

Los logs se escriben con el siguiente formato 
05-05-2023 15:55:14 [Information] {"IdServicio":4,"Servicio":"ObtenerProductosPorPaginacion","Usuario":"string","Legajo":"13","Ip":"190.220.133.170","Proceso":"Productos","Fecha":"2023-05-05T15:55:13.730605-03:00","Duracion":"00:00:00.6205778","Estado":"ok"













