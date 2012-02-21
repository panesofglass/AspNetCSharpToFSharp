(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,WebSharper,Html,Operators,Default,List,T,jQuery,Formlet,Data,Formlet1,Website,Forms,Controls,Enhance,FormButtonConfiguration,Remoting,window,FormContainerConfiguration,Control,_FSharpEvent_1,Formlet2,Base,_Result_1,Concurrency,Client;
 Runtime.Define(Global,{
  Website:{
   Client:{
    HelloWorld:function()
    {
     var x,f,f1;
     x=Operators.add(Default.Div(List.ofArray([Default.Id("message")])),Runtime.New(T,{
      $:0
     }));
     f=(f1=function(self)
     {
      var x1,f2;
      x1=jQuery.getJSON("HelloWorld",Runtime.Tupled(function(tupledArg)
      {
       var data,_arg1;
       data=tupledArg[0];
       _arg1=tupledArg[1];
       return self.set_Text(data.Message);
      }));
      f2=function(value)
      {
       value;
      };
      return f2(x1);
     },function(w)
     {
      return Operators.OnAfterRender(f1,w);
     });
     f(x);
     return x;
    }
   },
   Forms:{
    BasicInfoForm:function()
    {
     var x;
     return Data.$(Data.$((x=function(name)
     {
      return function(age)
      {
       return{
        Name:name,
        Age:age
       };
      };
     },Formlet1.Return(x)),Forms.Input("Name","Please enter your name")),Forms.InputInt("Age","Please enter a valid age"));
    },
    ContactInfoForm:function()
    {
     var phone,x,f,f1,address,x1,_builder_;
     phone=(x=Forms.Input("Phone","Empty phone number not allowed"),(f=(f1=function(arg0)
     {
      return{
       $:0,
       $0:arg0
      };
     },function(formlet)
     {
      return Formlet1.Map(f1,formlet);
     }),f(x)));
     address=Data.$(Data.$(Data.$((x1=function(str)
     {
      return function(cty)
      {
       return function(ctry)
       {
        return{
         $:1,
         $0:{
          Street:str,
          City:cty,
          Country:ctry
         }
        };
       };
      };
     },Formlet1.Return(x1)),Forms.Input("Street","Empty street not allowed")),Forms.Input("City","Empty city not allowed")),Forms.Input("Country","Empty country not allowed"));
     _builder_=Formlet1.Do();
     return _builder_.Delay(function()
     {
      var x2,x3,f2,f3;
      return _builder_.Bind((x2=(x3=List.ofArray([["Phone",phone],["Address",address]]),(f2=function(vls)
      {
       return Controls.Select(0,vls);
      },f2(x3))),(f3=function(formlet)
      {
       return Enhance.WithTextLabel("Contact Method",formlet);
      },f3(x2))),function(_arg1)
      {
       return _builder_.ReturnFrom(_arg1);
      });
     });
    },
    Input:function(label,err)
    {
     var x,x1,x2,f,f1,f2;
     x=(x1=(x2=Controls.Input(""),(f=function(arg10)
     {
      return Data.Validator().IsNotEmpty(err,arg10);
     },f(x2))),(f1=function(formlet)
     {
      return Enhance.WithValidationIcon(formlet);
     },f1(x1)));
     f2=function(formlet)
     {
      return Enhance.WithTextLabel(label,formlet);
     };
     return f2(x);
    },
    InputInt:function(label,err)
    {
     var x,x1,x2,x3,f,f1,f2,f3,f4;
     x=(x1=(x2=(x3=Controls.Input(""),(f=Data.Validator().IsInt(err),f(x3))),(f1=function(formlet)
     {
      return Enhance.WithValidationIcon(formlet);
     },f1(x2))),(f2=function(formlet)
     {
      return Enhance.WithTextLabel(label,formlet);
     },f2(x1)));
     f3=(f4=function(value)
     {
      return value<<0;
     },function(formlet)
     {
      return Formlet1.Map(f4,formlet);
     });
     return f3(x);
    },
    LoginForm:function(redirectUrl)
    {
     var uName,x,x1,x2,f,f1,f2,pw,x3,x4,x5,f3,f4,f5,loginF,x6,x7,_builder_,f8;
     uName=(x=(x1=(x2=Controls.Input(""),(f=function(arg10)
     {
      return Data.Validator().IsNotEmpty("Enter Username",arg10);
     },f(x2))),(f1=function(formlet)
     {
      return Enhance.WithValidationIcon(formlet);
     },f1(x1))),(f2=function(formlet)
     {
      return Enhance.WithTextLabel("Username",formlet);
     },f2(x)));
     pw=(x3=(x4=(x5=Controls.Password(""),(f3=function(arg10)
     {
      return Data.Validator().IsNotEmpty("Enter Password",arg10);
     },f3(x5))),(f4=function(formlet)
     {
      return Enhance.WithValidationIcon(formlet);
     },f4(x4))),(f5=function(formlet)
     {
      return Enhance.WithTextLabel("Password",formlet);
     },f5(x3)));
     loginF=Data.$(Data.$((x6=function(n)
     {
      return function(pw1)
      {
       return{
        Name:n,
        Password:pw1
       };
      };
     },Formlet1.Return(x6)),uName),pw);
     x7=(_builder_=Formlet1.Do(),_builder_.Delay(function()
     {
      var f6,submitConf,inputRecord,resetConf,inputRecord1;
      return _builder_.Bind((f6=(submitConf=(inputRecord=FormButtonConfiguration.get_Default(),Runtime.New(FormButtonConfiguration,{
       Label:{
        $:1,
        $0:"Login"
       },
       Style:inputRecord.Style,
       Class:inputRecord.Class
      })),(resetConf=(inputRecord1=FormButtonConfiguration.get_Default(),Runtime.New(FormButtonConfiguration,{
       Label:{
        $:1,
        $0:"Reset"
       },
       Style:inputRecord1.Style,
       Class:inputRecord1.Class
      })),function(formlet)
      {
       return Enhance.WithCustomSubmitAndResetButtons(submitConf,resetConf,formlet);
      })),f6(loginF)),function(_arg6)
      {
       var a,f7;
       return _builder_.ReturnFrom((a=Remoting.Async("Website:0",[_arg6]),(f7=function(loggedIn)
       {
        var _;
        if(loggedIn)
         {
          _=window;
          _.location=redirectUrl;
          redirectUrl;
          return Formlet1.Return(null);
         }
        else
         {
          return Forms.WarningPanel("Login failed");
         }
       },Forms.WithLoadingPane(a,f7))));
      });
     }));
     f8=function(formlet)
     {
      return Enhance.WithFormContainer(formlet);
     };
     return f8(x7);
    },
    SignupSequence:function()
    {
     var infoForm,x,x1,f,f1,fc,inputRecord,Description,x2,f2,f3,x3,f4,f5,contactForm,x4,x5,f6,f7,fc1,inputRecord1,Description1,x6,f8,f9,x7,fa,fb,proc,xe,_builder_,f10;
     infoForm=(x=(x1=Forms.BasicInfoForm(),(f=function(formlet)
     {
      return Enhance.WithSubmitAndResetButtons(formlet);
     },f(x1))),(f1=(fc=(inputRecord=FormContainerConfiguration.get_Default(),(Description=(x2=(f2=function(arg0)
     {
      return{
       $:0,
       $0:arg0
      };
     },f2("Please enter your name and age below.")),(f3=function(arg0)
     {
      return{
       $:1,
       $0:arg0
      };
     },f3(x2))),Runtime.New(FormContainerConfiguration,{
      Header:(x3=(f4=function(arg0)
      {
       return{
        $:0,
        $0:arg0
       };
      },f4("Step 1 - Your name and age")),(f5=function(arg0)
      {
       return{
        $:1,
        $0:arg0
       };
      },f5(x3))),
      Padding:inputRecord.Padding,
      Description:Description,
      BackgroundColor:inputRecord.BackgroundColor,
      BorderColor:inputRecord.BorderColor,
      CssClass:inputRecord.CssClass,
      Style:inputRecord.Style
     }))),function(formlet)
     {
      return Enhance.WithCustomFormContainer(fc,formlet);
     }),f1(x)));
     contactForm=(x4=(x5=Forms.ContactInfoForm(),(f6=function(formlet)
     {
      return Enhance.WithSubmitAndResetButtons(formlet);
     },f6(x5))),(f7=(fc1=(inputRecord1=FormContainerConfiguration.get_Default(),(Description1=(x6=(f8=function(arg0)
     {
      return{
       $:0,
       $0:arg0
      };
     },f8("Please enter your phone number or your address below.")),(f9=function(arg0)
     {
      return{
       $:1,
       $0:arg0
      };
     },f9(x6))),Runtime.New(FormContainerConfiguration,{
      Header:(x7=(fa=function(arg0)
      {
       return{
        $:0,
        $0:arg0
       };
      },fa("Step 2 - Your preferred contact information")),(fb=function(arg0)
      {
       return{
        $:1,
        $0:arg0
       };
      },fb(x7))),
      Padding:inputRecord1.Padding,
      Description:Description1,
      BackgroundColor:inputRecord1.BackgroundColor,
      BorderColor:inputRecord1.BorderColor,
      CssClass:inputRecord1.CssClass,
      Style:inputRecord1.Style
     }))),function(formlet)
     {
      return Enhance.WithCustomFormContainer(fc1,formlet);
     }),f7(x4)));
     proc=function(info)
     {
      return function(contact)
      {
       return function()
       {
        var result,phone,address,x8,x9,_this,xa,fd,xc,fe,xd,ff,_this1;
        result=contact.$==0?(phone=contact.$0,"the phone number: "+phone):(address=contact.$0,"the address: "+address.Street+", "+address.City+", "+address.Country);
        x8=List.ofArray([(x9=List.ofArray([Default.Text("Sign-up summary")]),(_this=Default.Tags(),_this.NewTag("legend",x9))),Default.P(List.ofArray([(xa="Hi "+info.Name+"!",(fd=function(xb)
        {
         return Default.Text(xb);
        },fd(xa)))])),Default.P(List.ofArray([(xc="You are "+Global.String(info.Age)+" years old",(fe=function(xb)
        {
         return Default.Text(xb);
        },fe(xc)))])),Default.P(List.ofArray([(xd="Your preferred contact method is via "+result,(ff=function(xb)
        {
         return Default.Text(xb);
        },ff(xd)))]))]);
        _this1=Default.Tags();
        return _this1.NewTag("fieldset",x8);
       };
      };
     };
     xe=(_builder_=Formlet1.Do(),_builder_.Delay(function()
     {
      return _builder_.Bind(infoForm,function(_arg3)
      {
       return _builder_.Bind(contactForm,function(_arg2)
       {
        return _builder_.ReturnFrom(Formlet1.OfElement((proc(_arg3))(_arg2)));
       });
      });
     }));
     f10=function(formlet)
     {
      return Formlet1.Flowlet(formlet);
     };
     return f10(xe);
    },
    WarningPanel:function(label)
    {
     var _builder_;
     _builder_=Formlet1.Do();
     return _builder_.Delay(function()
     {
      var genElem;
      return _builder_.Bind((genElem=function()
      {
       return Operators.add(Default.Div(List.ofArray([Default.Attr().Class("warningPanel")])),List.ofArray([Default.Text(label)]));
      },Formlet1.OfElement(genElem)),function()
      {
       return _builder_.ReturnFrom(Formlet1.Never());
      });
     });
    },
    WithLoadingPane:function(a,f)
    {
     var loadingPane,f1;
     loadingPane=(f1=function()
     {
      var elem,state,x,f2,f4;
      elem=Default.Div(List.ofArray([Default.Attr().Class("loadingPane")]));
      state=_FSharpEvent_1.New();
      x=(f2=function()
      {
       var f3;
       f3=function(_arg5)
       {
        var x1;
        x1=Runtime.New(_Result_1,{
         $:0,
         $0:_arg5
        });
        state.event.Trigger(x1);
        return Concurrency.Return(null);
       };
       return Concurrency.Bind(a,f3);
      },Concurrency.Delay(f2));
      f4=function(arg00)
      {
       var t;
       t={
        $:0
       };
       return Concurrency.Start(arg00);
      };
      f4(x);
      return[elem,function(value)
      {
       value;
      },state.event];
     },Formlet1.BuildFormlet(f1));
     return Formlet1.Replace(loadingPane,f);
    }
   },
   HelloWorldControl:Runtime.Class({
    get_Body:function()
    {
     return Client.HelloWorld();
    }
   }),
   LoginControl:Runtime.Class({
    get_Body:function()
    {
     return Forms.LoginForm(this.redirectUrl);
    }
   }),
   SignupSequenceControl:Runtime.Class({
    get_Body:function()
    {
     return Forms.SignupSequence();
    }
   })
  }
 });
 Runtime.OnInit(function()
 {
  WebSharper=Runtime.Safe(Global.IntelliFactory.WebSharper);
  Html=Runtime.Safe(WebSharper.Html);
  Operators=Runtime.Safe(Html.Operators);
  Default=Runtime.Safe(Html.Default);
  List=Runtime.Safe(WebSharper.List);
  T=Runtime.Safe(List.T);
  jQuery=Runtime.Safe(Global.jQuery);
  Formlet=Runtime.Safe(WebSharper.Formlet);
  Data=Runtime.Safe(Formlet.Data);
  Formlet1=Runtime.Safe(Formlet.Formlet);
  Website=Runtime.Safe(Global.Website);
  Forms=Runtime.Safe(Website.Forms);
  Controls=Runtime.Safe(Formlet.Controls);
  Enhance=Runtime.Safe(Formlet.Enhance);
  FormButtonConfiguration=Runtime.Safe(Enhance.FormButtonConfiguration);
  Remoting=Runtime.Safe(WebSharper.Remoting);
  window=Runtime.Safe(Global.window);
  FormContainerConfiguration=Runtime.Safe(Enhance.FormContainerConfiguration);
  Control=Runtime.Safe(WebSharper.Control);
  _FSharpEvent_1=Runtime.Safe(Control["FSharpEvent`1"]);
  Formlet2=Runtime.Safe(Global.IntelliFactory.Formlet);
  Base=Runtime.Safe(Formlet2.Base);
  _Result_1=Runtime.Safe(Base["Result`1"]);
  Concurrency=Runtime.Safe(WebSharper.Concurrency);
  return Client=Runtime.Safe(Website.Client);
 });
 Runtime.OnLoad(function()
 {
 });
}());
