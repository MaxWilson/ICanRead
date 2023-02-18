# ICanRead

Flash cards to help children learn phonics and reading

    dotnet tool restore
    npm install
    npm install -g @azure/static-web-apps-cli azure-functions-core-tools@4

To On three separate command lines, start Fable, the az function, and swa emulator which ties them both together.
    start swa start http://[::1]:3000 --api-location http://127.0.0.1:7071 --run "npm start" && cd api && start func start --csharp
    
Alternatively you can run them all separately:
    
    cd api && func start --csharp         
    swa start http://localhost:3000 --api-location http://localhost:7071 --run "npm start"
    
    REM (Yes, that --csharp is deliberate, and func will still load the .fsproj correctly.)

Then navigate to http://localhost:4280 to use the app. Both fable and swa support hot-reloading so everything should just work.

See https://github.com/aaronpowell/swa-feliz-template for more build and deployment instructions.

# Whitespace errors

If you see superfluous ^Ms in git diff, do git config core.whitespace cr-at-eol