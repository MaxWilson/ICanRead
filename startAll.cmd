pushd api && start npx func start --csharp
popd
start swa start http://localhost:3000 --api-location http://127.0.0.1:7071 --run "npm start"
