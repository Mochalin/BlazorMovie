function my_function(message){
    console.log("from utilities " + message);
}
function dotNetStaticInvocation(){
    DotNet.invokeMethodAsync("BlazorDemoUdemy.Client","GetCurrentCount")
    .then(result=>{
        console.log("Count from java script" + result);
    })
}
function dotNetInstanceInvocation(dotNetHelper){
    dotNetHelper.invokeMethodAsync("IncrementCount")
}