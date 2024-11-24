import pandas as pd
import json

# 엑셀 파일 로드
df = pd.read_excel('파일명.xlsx')

# DataFrame을 JSON 형식으로 변환
data = df.to_dict(orient='records')

# JSON 파일로 저장
with open('data.json', 'w', encoding='utf-8') as f:
    json.dump(data, f, ensure_ascii=False, indent=4)
