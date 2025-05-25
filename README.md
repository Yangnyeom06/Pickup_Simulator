# Pickup_Simulator

## UI 진행현황
- [X] upgrade --> 2차 수정 완료
- [X] gameStart --> 1차 수정 필요
- [X] gameCause --> 1차 수정 필요
- [ ] Ingame --> 1차 수정 필요
- [ ] setting --> 1차 수정 필요


## 피그마 디자인 가져오기

### UnityFigmaBridge 패키지 설치
1. Window -> Project Manager -> + -> Add package from git URL...
2. https://github.com/simonoliver/UnityFigmaBridge.git 입력

### TextMeshPro 패키지 설치 및 TMP 필수 리소스 가져오기
1. Window > TextMeshPro -> import TMP Essential Resource
2. import 선택

### Unity Figma Bridge 설정 에셋 생성
1. Edit -> Project Setting ->  Unity Figma Bridge -> create (설정 에셋 생성)
2. Document Url에 Figma Url 입력
3. Figma Bridge -> Set Personal Access Token
4. 자신의 피그마 계정에서 Setting -> Security -> Generate new token
5. Scopes에서 File content는 Read-Only, Comments/Dev resource/Webbooks는 write으로 설정
6. 생성한 토큰을 유니티의 Personal Access Token에 입력

### 피그마 문서 가져오기
1. Figma Bridge -> Sync Document
2. Assets에 FigmaOutput 파일과 Figma 폴더가 생성되었는지 확인

### 유니티에서 피그마UI 확인하기
1. Hierarcy -> Canvas -> ScreenParentTransform
2. ______Screen 추가
