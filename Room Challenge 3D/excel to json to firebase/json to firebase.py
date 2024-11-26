import firebase_admin
from firebase_admin import credentials, db
import json
import pandas as pd

# Firebase 초기화
cred = credentials.Certificate("파일명.json")
firebase_admin.initialize_app(cred, {
    'databaseURL': 'DB주소'
})

# JSON 파일 읽기
with open('data.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

# NaN 값을 None으로 대체 (JSON 직렬화 가능)
# pandas의 DataFrame에서 NaN을 처리
for record in data:
    for key, value in record.items():
        if value != value:  # NaN 값은 자기 자신과 비교하면 False이므로, NaN인지 확인
            record[key] = None  # NaN을 None으로 대체

# 데이터베이스 참조
ref = db.reference('이름')

# 데이터를 Firebase에 업로드
ref.set(data)
