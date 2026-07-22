# LuaScriptEngineLib

C#/.NET에서 Lua 스크립트를 실행하기 위한 엔진 라이브러리입니다. [NeoLua](https://github.com/neolithos/neolua)를 기반으로 하며, 팩토리·바인딩·샌드박스·취소를 제공합니다.

## 주요 기능

- **스크립트 실행**: `ILuaScriptEngine`의 `Eval` / `EvalAsync`로 Lua 소스 실행
- **C# ↔ Lua 바인딩**: `LuaScriptEngineFactory`에 `AddFunction` / `AddLibrary`로 전역 함수·라이브러리 등록, `GetFunction`으로 Lua 함수를 형식 안전하게 호출
- **출력 연동**: `ILuaScriptEngineOutputEmitter`로 `print` 등 스크립트 출력을 호스트에 전달
- **샌드박스(기본 활성)**: 파일·OS·패키지·디버그·동적 로드 등 위험 API와 CLR 접근을 차단
- **실행 취소**: `CancellationToken`으로 실행 중 스크립트를 중단

## 사용 흐름

1. `LuaScriptEngineFactory` 생성 후 필요 시 함수·라이브러리 등록
2. `CreateEngine`으로 `ILuaScriptEngine` 생성
3. `Eval` / `EvalAsync`로 스크립트 실행 (필요 시 `GetFunction`으로 Lua 함수 호출)
4. 사용 후 `Dispose`

## 대상

- 대상 프레임워크: `net10.0`
- 의존성: `NeoLuaDebug`
