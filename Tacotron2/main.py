from fastapi import FastAPI
from fastapi.responses import StreamingResponse
from pydub import AudioSegment
import io
import time

import torchaudio
from speechbrain.pretrained import Tacotron2
from speechbrain.pretrained import HIFIGAN

app = FastAPI()

# Инициализация TTS (Tacotron2) и Vocoder (HiFi-GAN)
tacotron2 = Tacotron2.from_hparams(source="speechbrain/tts-tacotron2-ljspeech")
hifi_gan = HIFIGAN.from_hparams(source="speechbrain/tts-hifigan-ljspeech")

#time.sleep(30) 

@app.get("/")
def get_audio(query):
    a = time.time()
    mel_output, mel_length, alignment = tacotron2.encode_text(query)

    # Запуск Vocoder (спектрограмма в аудиофайл)
    waveforms = hifi_gan.decode_batch(mel_output)

    # Преобразование в формат MP3 в памяти
    audio_data = io.BytesIO()
    torchaudio.save(audio_data, waveforms.squeeze(1), 22050, format='wav')
    audio_data.seek(0)
    audio_segment = AudioSegment.from_wav(audio_data)
    mp3_data = io.BytesIO()
    audio_segment.export(mp3_data, format="mp3")

    # Перемотка указателя в начало данных
    audio_data.seek(0)

    # Отправка MP3-данных в качестве ответа
    return StreamingResponse(audio_data, media_type="audio/mpeg", headers={"Content-Disposition": f'attachment; filename="Pronunciation.mp3"'})

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)