cmake_minimum_required(VERSION 3.20)
project(ToDoServerCpp2 CXX)

set(CMAKE_CXX_STANDARD 17)

# gRPC та protobuf
#find_package(protobuf CONFIG REQUIRED)
#find_package(gRPC CONFIG REQUIRED)
#find_package(protobuf CONFIG REQUIRED)

# proto-файли
set(PROTO_SRC_DIR "${CMAKE_CURRENT_SOURCE_DIR}/proto")
set(PROTO_GEN_DIR "${CMAKE_CURRENT_BINARY_DIR}/generated")

file(MAKE_DIRECTORY ${PROTO_GEN_DIR})

protobuf_generate_cpp(PROTO_SRCS PROTO_HDRS ${PROTO_SRC_DIR}/todo.proto)
protobuf_generate_grpc_cpp(GRPC_SRCS GRPC_HDRS ${PROTO_SRC_DIR}/todo.proto)

add_executable(ToDoServerCpp2
    src/main.cpp
    src/server.cpp
    ${PROTO_SRCS}
    ${PROTO_HDRS}
    ${GRPC_SRCS}
    ${GRPC_HDRS}
)

target_include_directories(ToDoServerCpp2 PRIVATE ${PROTO_GEN_DIR} ${PROTO_SRC_DIR})
target_link_libraries(ToDoServerCpp2 PRIVATE gRPC::grpc++ protobuf::libprotobuf)

cmake_minimum_required(VERSION 3.20)
project(MyGRPCApp)

# Додати support vcpkg
set(CMAKE_TOOLCHAIN_FILE "${CMAKE_SOURCE_DIR}/vcpkg/scripts/buildsystems/vcpkg.cmake"
    CACHE STRING "Vcpkg toolchain file")

find_package(Protobuf REQUIRED)
find_package(gRPC REQUIRED)

add_executable(my_app main.cpp)

target_link_libraries(my_app
    PRIVATE
    gRPC::grpc++
    protobuf::libprotobuf
)
