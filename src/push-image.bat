aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 993530973844.dkr.ecr.us-east-1.amazonaws.com
ods\src> docker build -f "ods.extractor/Dockerfile" -t dev-theradexodsextractor . 
docker tag dev-theradexodsextractor:latest 993530973844.dkr.ecr.us-east-1.amazonaws.com/dev-theradexodsextractor:dev-latest
docker push 993530973844.dkr.ecr.us-east-1.amazonaws.com/dev-theradexodsextractor:dev-latest