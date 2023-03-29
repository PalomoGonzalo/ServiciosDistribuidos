Este proyecto se centra en varias aplicaciones que todos se comunican entre si,

Hasta el momento se encuentra el UserManager que es el servicio que se encarga el tema de logueo y altas de usuarios, perfiles.
Actualmente se encuentra online a traves de un servidor vps con docker y mysql como base de datos http://89.117.32.214:5025/swagger/index.html.
Para poder utilizar los endpoints se necesita tener el jwt que se obtiene al loguear 

![image](https://user-images.githubusercontent.com/89616271/225304602-d3a4fdf7-8942-4dcf-8a37-8db31b65c3dc.png)

Te devuelve el token

![image](https://user-images.githubusercontent.com/89616271/225304902-86c00350-8ced-481f-ab6a-eb82da5f9437.png)

Y en authorize escribir Bearer + token 

![image](https://user-images.githubusercontent.com/89616271/225305091-fd5d73ab-311b-4441-a8e0-4aa9c3c12142.png)

Los endpoints son cruds, con baja logica, contaseñas hassheadas, cada accion que se realiza se guarda eventos, como cambiar contraseña, registrar usuario, eliminar usuario obteniendo la ip de quien lo realizo


