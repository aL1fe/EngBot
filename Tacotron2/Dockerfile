FROM python:latest

WORKDIR /app

COPY . .

RUN python -m pip install --upgrade pip
RUN apt-get update 
RUN apt-get install -y ffmpeg
RUN rm -rf /var/lib/apt/lists/*

RUN pip install -r requirements.txt

CMD [ "python", "main.py" ]