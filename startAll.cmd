pushd api && start npx func start --csharp
popd
start npm start
start npx swa start http://127.0.0.1:3000 --api-location http://127.0.0.1:7071
