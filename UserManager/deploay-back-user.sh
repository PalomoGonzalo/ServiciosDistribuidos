#!/bin/bash
appName="usermanager-backend-api"
appSourceDir="./docker_home/ServiciosDistribuidos/"
startTime=`date +%s`



# Nos movemos al directorio donde se encuentra el repositorio
cd $appSourceDir

echo "Updating local repository..."
# Se pullean los cambios del repo para el branch indicado
git pull

cd ./UserManager

# Se obtiene la version desde el csproj
version=$(sed -n 's:.*<AssemblyVersion>\(.*\)</AssemblyVersion>.*:\1:p' UserManager.csproj)

echo "($version)"

# Si ya hay un container corriendo para esta aplicacion se lo elimina
if [ "$(docker ps -a | grep $appName)" ]; then
        echo "Removing container..."
        docker stop $appName
        docker rm $appName
fi

echo "Building image..."

# Se crea la imagen para el ambiente indicado, y se la tagea con la versi  n
echo "Building image (v$version)..."
docker build -t $appName:$version .

# Se levanta el container de la aplicacion

echo "Running container..."
docker run --name $appName -d -p 5025:80 $appName:$version


# Limpio containers e imagenes huerfanos
docker container prune -f
docker image prune -f

endTime=`date +%s`

# Imprimo tiempo transcurrido
echo "Total time: $((endTime-startTime))"

