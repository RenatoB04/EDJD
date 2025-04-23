cmake_minimum_required(VERSION 3.31)
project(P01_P3D)

set(CMAKE_CXX_STANDARD 20)

set(CMAKE_TOOLCHAIN_FILE "C:/Users/Acer/vcpkg/scripts/buildsystems/vcpkg.cmake" CACHE STRING "")

add_executable(P01_P3D src/main.cpp)

find_package(OpenGL REQUIRED)
find_package(glfw3 CONFIG REQUIRED)
find_package(GLEW REQUIRED)

target_link_libraries(P01_P3D
        PRIVATE
        OpenGL::GL
        glfw
        GLEW::GLEW
)

target_include_directories(P01_P3D PRIVATE ${VCPKG_INCLUDE_DIRS})