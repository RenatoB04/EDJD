# Projeto - Programação 3D

Este projeto implementa uma simulação 3D interativa de uma mesa de bilhar com 15 bolas, com iluminação, controlo de câmara, minimapa e animação física básica. Desenvolvido no âmbito da unidade curricular de **Programação 3D**.

---

## Requisitos e Instalação

### 1. Clonar e instalar dependências com o `vcpkg`:

```bash
git clone https://github.com/microsoft/vcpkg.git
cd vcpkg
.\bootstrap-vcpkg.bat
./vcpkg install glew glfw3 glm stb
```

### 2. Adicionar o `vcpkg.cmake` ao `CMakeLists.txt`:

No topo do ficheiro `CMakeLists.txt`, acrescentar:

```cmake
set(CMAKE_TOOLCHAIN_FILE "C:/Users/Acer/vcpkg/scripts/buildsystems/vcpkg.cmake")
```

> Substituir o caminho pelo que corresponder à instalação local do `vcpkg`.

---

## Dependências

O projeto utiliza as seguintes bibliotecas externas:

* **[GLEW](https://glew.sourceforge.net/):** OpenGL Extension Wrangler Library — para carregar extensões OpenGL.
* **[GLFW](https://www.glfw.org/):** Biblioteca para criação de janelas e gestão de eventos.
* **[GLM](https://glm.g-truc.net/):** Biblioteca de matemática orientada a gráficos 3D.
* **[stb\_image.h](https://github.com/nothings/stb):** Biblioteca para carregamento de imagens/texturas.

---

## Licenças

* **GLEW, GLFW, GLM:** Licenciadas de acordo com os termos originais (consultar os respetivos sites).
* **stb\_image.h:** Domínio público / Licença MIT.

---

## 🔧 Controlo via Teclado e Rato

| Tecla     | Função                                                      |
| --------- | ----------------------------------------------------------- |
| `1`       | Ativar/desativar **luz ambiente**                           |
| `2`       | Ativar/desativar **luz direcional**                         |
| `3`       | Ativar/desativar **luz pontual**                            |
| `4`       | Ativar/desativar **luz cónica (spotlight)**                 |
| `5`       | Ativar/desativar **modo sem iluminação** (flat shading)     |
| `Mouse 2` | Rodar a câmara em torno da mesa                             |
| `Scroll`  | Aproximar/afastar (zoom)                                    |
| `Espaço`  | Iniciar movimento da bola principal (com rotação e colisão) |

---

## Funcionalidade e Estrutura

Este projeto cumpre os requisitos dos **Passos 1 a 4** do enunciado:

### Passo 1 — Base Gráfica

* Janela e interações via **GLFW**
* Matrizes e transformações com **GLM**
* Mesa de bilhar representada como paralelepípedo via **OpenGL (GLEW)**
* Faces com cores distintas (sem textura inicial)
* **Scroll** do rato aplica zoom; **movimento do rato** permite rotação da câmara
* **Minimapa** (vista superior) no canto superior direito, independente da iluminação

---

### Passo 2 — Biblioteca Orientada a Objetos

* Biblioteca `RendererLib` com:

  * `Model::Load()` → lê `.obj` e `.mtl`, carrega a textura associada
  * `Model::Install()` → envia vértices, normais e UVs para a GPU
  * `Model::Render(pos, orient)` → desenha o modelo com transformação
* Suporte a ficheiros `.obj`, `.mtl` e texturas via **stb\_image**
* 15 bolas carregadas e posicionadas numa disposição predefinida

---

### Passo 3 — Iluminação Interativa

* Quatro tipos de luz implementados:

  * Ambiente
  * Direcional
  * Pontual
  * Cónica (Spotlight)
* Cada tipo pode ser ativado/desativado com as teclas `1` a `4`
* O `shader.frag` adapta-se dinamicamente ao estado de cada tipo de luz
* Iluminação calculada em tempo real (modelo de Phong simplificado)

---

### Passo 4 — Animação da Bola

* Ao premir **Espaço**:

  * A bola principal desloca-se e roda
  * É detetada colisão com outras bolas (por distância)
  * O movimento termina se houver colisão ou se sair dos limites
* Movimento baseado em **deltaTime** para garantir fluidez

---

## Créditos

Projeto desenvolvido por:

* Paulo Bastos — 27945
* Bruno Mesquita — 27947
* Ricardo Miranda — 27927
* Bento Simões — 27914
