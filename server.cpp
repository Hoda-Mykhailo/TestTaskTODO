#include <iostream>
#include <memory>
#include <string>
#include <vector>
#include <grpcpp/grpcpp.h>
#include "todo.grpc.pb.h"

using grpc::Server;
using grpc::ServerBuilder;
using grpc::ServerContext;
using grpc::Status;
using todo::ToDoService;
using todo::TaskRequest;
using todo::TaskResponse;
using todo::Empty;
using todo::Task;
using todo::TaskList;

std::vector<std::string> tasks;

class ToDoServiceImpl final : public todo::ToDoService::Service {
    Status AddItem(ServerContext* context, const AddRequest* request, AddResponse* response) override {
        static int id = 0;
        id++;

        todo::ToDoItem* item = response->mutable_item();
        item->set_id(id);
        item->set_description(request->description());
        item->set_completed(false);

        std::cout << "Added item: " << item->description() << std::endl;

        return Status::OK;
    }

    Status UpdateStatus(ServerContext* context, const UpdateRequest* request, UpdateResponse* response) override {
        todo::ToDoItem* item = response->mutable_item();
        item->set_id(request->id());
        item->set_description("Updated task"); // Заглушка
        item->set_completed(true);

        std::cout << "Updated item with id: " << request->id() << std::endl;

        return Status::OK;
    }

    Status Subscribe(ServerContext* context, const Empty* request, grpc::ServerWriter<todo::ToDoItem>* writer) override {
        for (int i = 0; i < 5; ++i) {
            todo::ToDoItem item;
            item.set_id(i);
            item.set_description("Task " + std::to_string(i));
            item.set_completed(false);
            writer->Write(item);
        }
        return Status::OK;
    }
};
