## 개요

- 개발 기간
    - 2024.11.1 ~12.1
- 개발 인원
    - 1인
- 목표 : 확장성, 유지 보수를 고려한 API 설계
- 목적 : Unity 클라이언트에서 유저 데이터를 저장하고 관리할 수 있는 백엔드 구축
- Github 주소
    - 클라이언트 - https://github.com/Jeonhyeongwoo1/SlimeMaster
    - 서버 - https://github.com/Jeonhyeongwoo1/slime_master_server
 
### [ASP.NET](http://ASP.NET)  API 요청 처리 흐름
<div align="center">
    <img width="936" alt="image (11)" src="https://github.com/user-attachments/assets/aa4af119-b98b-43f1-b6a0-12a5afa81623" />
</div>

- Client :
    - API 요청
    - 요청 유형: `POST`
    - 데이터: `JSON 형식 (UserID, Token, Request Body 등)`
    - JWT 포함
- Controller :
    - API에 맞는 요청 실행(유저 데이터 조회, 요청 등)
    - 비즈니스 로직 실행 요청
- Serivce :
    - 데이터베이스 접근
    - 유저 데이터 가공 및 읽기, 쓰기
- Firebase :
    - 유저 데이터 저장
