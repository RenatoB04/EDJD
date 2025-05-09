# Projeto - Programação 3D

Este projeto implementa uma simulação 3D interativa de uma mesa de bilhar com 15 bolas, com iluminação, controlo de câmara, minimapa e animação física básica. Desenvolvido no âmbito da unidade curricular de **Programação 3D**.

---

## Requisitos e Instalação

### 1. Clonar e instalar dependências com `vcpkg`:

```bash
git clone https://github.com/microsoft/vcpkg.git
cd vcpkg
.\bootstrap-vcpkg.bat
./vcpkg install glew glfw3 glm stb
```

### 2. Adicionar `vcpkg.cmake` ao teu `CMakeLists.txt`:

No topo do ficheiro `CMakeLists.txt`, acrescenta:

```cmake
set(CMAKE_TOOLCHAIN_FILE "C:/Users/Acer/vcpkg/scripts/buildsystems/vcpkg.cmake")
```

Substitui o caminho pelo que corresponde à tua instalação local.

---

## 🔧 Teclas de controlo

| Tecla    | Função                                                           |
| -------- | ---------------------------------------------------------------- |
| `1`      | Ativar/desativar **luz ambiente**                                |
| `2`      | Ativar/desativar **luz direcional**                              |
| `3`      | Ativar/desativar **luz pontual**                                 |
| `4`      | Ativar/desativar **luz cónica**                                  |
| `5`      | Ativar/desativar **modo sem iluminação** (flat shading)          |
| `Mouse2` | Rodar a câmara em torno da mesa                                  |
| `Scroll` | Zoom in/out                                                      |
| `Espaço` | Iniciar movimento de uma bola (com rotação), que para ao colidir |

---

## Funcionalidade e Estrutura

Este projeto cumpre os requisitos dos **Passos 1 a 4** do enunciado:

### Passo 1 — Base gráfica

* Janela e interações via **GLFW**
* Matrizes e transformações via **GLM**
* Mesa de bilhar como paralelepípedo via **OpenGL (GLEW)**
* Faces com cores distintas (sem texturas inicialmente)
* **Scroll** do rato faz zoom, **movimento do rato** roda a vista
* **Minimapa** (top view) no canto superior direito, independente da iluminação

---

### Passo 2 — Biblioteca orientada a objetos

* Biblioteca `RendererLib` com:

  * `Model::Load()` → lê `.obj` e `.mtl`, carrega textura
  * `Model::Install()` → envia vértices, normais, UVs para GPU
  * `Model::Render(pos, orient)` → desenha com transformação
* Suporte a `.obj`, `.mtl`, e texturas com `stb_image`
* 15 bolas carregadas e posicionadas numa disposição definida

---

### Passo 3 — Iluminação interativa

* 4 tipos de luz implementadas:

  * Ambiente
  * Direcional
  * Pontual
  * Cónica
* Cada uma pode ser ativada/desativada com tecla (`1` a `4`)
* `shader.frag` recebe estados booleanos para desenhar consoante ativação
* Cálculos de luz feitos em tempo real (Phong simplificado)

---

### Passo 4 — Animação de bola

* Ao premir **Espaço**:

  * A bola `balls[0]` move-se e roda
  * Deteta colisões com outras bolas (distância)
  * Para se tocar em outra bola ou sair dos limites
* Movimento é baseado em **deltaTime** para fluidez

---

## Créditos

Desenvolvido por:
- Paulo Bastos 27945
- Bruno Mesquita 27947
- Ricardo Miranda 27927
- Bento Simões 27914
