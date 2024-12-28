using Microsoft.Extensions.Hosting;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Models;

namespace TelegramAsDatabase.Samples;

internal class MyService : IHostedService
{
    private readonly ITDB _tdbService;

    public MyService(ITDB tdbService)
    {
        _tdbService = tdbService;
    }

    private class MyTestModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string BigString = "Rx80F%ofWR7n)N4O_i!Q3BoqQ@Q6sDh-f^Cb0$5wh)d(e9r^Mk--q(cL6@sqkrAe5OZ+rV+LOkPN+uxI2y4U63WPxZ+9I!fVb=Q2InWySnNC&s!)Kn%j3T)F+^jxR&m3OGR2O(rk-+mpbu8%#0O#0I#ASUG$HD2=GI6N6E60DdElr1ePf)kyr+m!Rz++zJrdz9nDUTeYkPmQTL=9@YMwL)WlZKlb*a@muh^5Va4QhrqZ9+vLOfA-yv*0Z&qrBKK-M3I#4OO)oelU0M77)afbRY7^1Y!@X1Vcxp6%)nF$P6hnZKs0oFQVe6yz__yPAiaL*Ny+Sq)tWfe^SC_yIEzPx=yoELL85HbH18yvaD0GqW&$AnZOPwL8H#W*!lEeySNLt^nw-5M2U7l$4MF1ndQ&LNt@Q(VLrR@I$sgzG--KD5=k8tJoMeH!_ZUxSMCk4AEafAHAt^NQd)Tu3^p*IMTRd9pf0$zdbEUyf6OOE0SmZRReX$FUya9ZS@Bnx!+ReE+D^TStLM)0oZ8OcwQV-q(s2FDMgpW3#lY(-DJqdxquFcbDFMFibbuOtq^wKZ+0T@yl*%8lSBRZcK8Vslv5BmfQ6WNrhmUBuMRog6TT0XyrMOP8cOjw#nF@rbwuGvB5uYI$bLFjf!1oPS-K00kIUoaIo)nRn6v8WWlg0HF^=mxFbxA^&#RU&VJ)ql!w_orgjseQ*z@yA#wI4b@lj=l2iZDZ&MZGKiZcbfGBHkV_F@t26hp$53X)t=*On)X-22MDxQnoCxNS6SV5t6%jO=5uYzhrg_MKell_f%y9O(UMpUSJ=js!cloOpUWG1+QEoOmCL+gZ+093%w_S!9+_18L_X)xaTFI9T^0&oc7*U^1L4iFRw@g@sqOfksX+fWznPgyZpkQV#Vm_doijZbtKao(55Jctzk%Cy@*aLharxdxC#$Ec_n^T+NlU&9zSkRUGUw)j^kCX!n=r1rss&0ZeRB+W_HwfN-@=wPL9$udV*ZD@Dra+xlTWm1$MlyNJ@_TObT%TW#=dQujyXnCHitq&F13wTj@4Al&!PZpAg0oLhb0MZGts#0!KN$v^Qk61i0Hbx^3MZ^+^QDEcFYQ($XmrM&DoGuAbB3=2T$(s@6TiUkW^1j%5N%mA2KAlE5PCgo6-lPdi*(YkIQFcAO_^dTZJ(PDXzZrntqE4fizLZu+gYjG#cawttYyC3^ufECZ9wYz2JXHI_ZcYqNtHldqVX@uilmHsG*ynLz*ylMx*$H9kWz@3Kyp3I9j)U3X1tPn2cf^&!@&Xhh8KNShX^oU!tT*39Ib5$^R@1HwG#x((%SuRcRp1K4d!Bz-$AAHR$Xe^Q#_MJKh3DeZRwsa==aZysdPdK7sXcSx3zJ^xexu)kY(CybyPWb)2oFgaAXJEi8^WM_dnRhqs1_MwAnI$B6rT2K5^bTW@nXM*263-04LG!1x*cnp)=6VH9pSgl)G7!tk&O^z*%sj%6o9Bd3-!^np1JboWuCd@Sy33TB4#GEb7vNNnbW(n$ihH6SBQerd0W-IzTufXcS0LI3VLCugI5rOlEuiT686KGAvxgi6CUPDzlytZB#=nb9G4I&uy2&KbKk$3%PxH=5n(ZiDO_yW7I&aAb97raf5YINS2cktNvyr2VxiZ1OKE^tcjORQRTpu=mS_@iq*U4^3VUIOz@SpV=&d4SadE&Y0x=l)Amw7E4^H%3kxpqjL^&Hk^lOHMd$sVK5KnywTNq9q0tY7j4Lj&T!q8wlyNlV$qIcUJ9&193$f7P2-AfClt=A-(E7T#tCbKv5HkTIc^R^JSvUxNc5X3QR7H4@A5BVqIqjBc6!xkkOAT_vO&*C9zNH5p7inARAlGYnT@N%ks2Aron+f-SbDM%KS5bhgCnN8HnqujdhZOJcccZh&2TCcA(kReCziLRh-UHHnFNAK-H-!#sz@5o$K9=eKqDR0t=zEqk$rNkZ1#BM7#v6+%s0-YMh*vHR+N1S3lEK7XBOV-Yk!x-rh_-0r&E(ntcIHl&$yeG1)YTpWOVklDmoJ3!eGEPS$=cCvJp#VXZHkLf&gq&3Q7+WiQ8S72h583xWsbuywe6aX)Nc=V^4+TYX-uVMOuDBtK&88=k=V1pmo!R#9pOUYswo2SC&qVV11r7d9lHEY469oIa%pBKWAGv-kBoRR)dFHBb1+^a4GF(PloCwAXfrz@!G*)nR96Bt2ABlH&=-)Z&@(h5V(0pASn9fmQr^YVb+Wj$DPwsfrgfkFiPl!azG2qxgfP6oWTBwGUufqAW@B-zGiCX&3O1@pXirbl@laA1pOOQaTHW27r#Z)hJ9VUA3XDRi$&ni4S*rX&k3nNGBt_@fuCkBmSw4o@h1L4BVGr8Tm8OjlvBaxo1U^qzen=MOh8rePfwJ3ut^We)k1@-IEXBXzaA_2q+FKN-ACuA!O5#GoT5UU15JfEbYJdgxCjK7uV1qnmMQY5TMA+js%tySwD2v^mKLI#iAYAkkMNzkvWC5beuSqgQMZPq%sa2a5C*illW=Un*O2DI0hQaRe3xECbhK93G1&Y2WH9ff&G2NBam-moMlR%m!TkMGZ&*@wNB1u4Ki0rb^tlIYN%h#*0!MbYh^h%EL@#AMrGPpCXOPBC_zjiFesen-OXqAaqx_zdTFp=E$JBR(u-fB@8W2$WxzFs8O)aBrF0q1YbUe&JMF=2x4QHQKOMAg#vP87vWU1IyrmnEL9l9Cpp8q(3$!l-W#FvKap(O8Mv!nQ0MMDB=9%=RIT-NsV3us%A)L=*5zs-fztbf)73h4DZla082jLY+Biu0tHt=oNHL0CDGPioIfmC$h@evFE_wIJ6PT&I29jB*t2oQB#c&$xg@XWLQNEtunjMnF_xQ3j+55*T-C=2a*X5DjXgh4Qc#7pORc)QdY(QaLidm7sfuH$6Gtx#_&i@CWrGI5uQVHQePxETKcoiK(Roi=3TmL5p)oq%L6@rXe8bBuB3s+CWQ&Cv^qfnO68&8i37#bX4b6O1l+owE&7QXFu=DV*=jRoejQ*oIu@13TRJ6nBgT*y$Z%L33Z9kXk^wlE$aO!*Os-LoHd8d(z4alUS9yiBc6Tt^KPevvacM+y*(tU4PGkCULVW64PJWGzlO#nHq73NrXUsIQV14+CeGgc2uI24aK#qBo9JUFHhnmVqZjXTCJHfbpg_Em3RI9_Y$1aOpjY*58hnebHLS9DQ8uM-%6*c(%98O@n*0c7@0T!a1NCh6P$QsgfQO1BB2QXvE*fSIV!3s26AEoBZW5ukp2*9-mvvEXqMbfG8sa0=xSKls4slp4SCjxuaDIYcn_*MU50Ss2y1zeFbqy_uOGLgKhfZ$CGfbU0WnDNr6%vP+*Y*+)GFKjL-e%)QHoOPPn+4%&sN#H$dI#i9!NfKCg0EQd5k23CO7oL*B-p9N4BkT(UNX^2^G^$tx#HpZNhs&+)mBDI+EpNPefIQ3a*ou3W5QEPlwrp$deU+L8a&LPZkFUV_l7&2hS)-iz_y&8g0oTgx2aBKEQJeY(nbIzkyX@(ta-S8K^pZCB7TK$gS&OnI6i8Td&IJ7*9lxd67!cBtFeoPXU!E(_oVYy%)UHz9eP##C_D62IFrK_kEr)c-6Zh*kU7-meD6g*&c62)EgO*rtT8bpHYg@uY5DYo6Gl^1n4LNEw$biS9lOyM%wY)MiAgIWWf4HOZm0f29-CgN$_SURK*jPIKx*skL_g2Jv7o@08_*3dOCL2qsrk8jS-fTb*9n1bY^#NmvBH%&1@kTw+uU6_w!UXjtffegQWM!bhkB^-Efhp#GP)-DmucY7P4lmqa-bkKoC*2#4BcSnCgR_GTZr+WM&@bIsxPOTXjDz1hffrrJBGhCTC1hRRG6a&dgS8$(=QVEjNnqW#pttjVWPEGLcD=J!uv&awNYk&Lf-)o3W&3@VQx8z5TDG*7_u)gnxM(Gtu9k+RRlYh^5W+Br80Du7Nm*Le^6@p2qufjxWu@@aSt#JOY@in%I&webSeTdC3ay!_Hn-pig+7(AUCOc&+*15B1E=kMxNjiUm_SqS48*+@AurV^Up($4dt4jFc-ze$*3PX*qV4i_SpH@1n!MWh9vChbweMuLLv*XtTRWU03wyQcMARgtWqY6)LA!f2l_C=r4!TVThTcsOHJS2I15-4BMHsewH-XpN7-i-R7-E!XTS4xz0-Z$Wvy9S-OavnIN40r4xyAjtulwJ^%xIUMuyekGQP$D6hPg25^+dSI!Mnad!u*TKjmA=WcW4R-+2=73sEBwx8@fNnWMpd=usmte@8b5gni$k1S-yIr!KRPa2^yqKGPU(b50j1@pdjM=V+X&fMvpxwiOVFQ0SO4J^G$F+SHM41%%R8T+0ReL@U+sWEhDE$s+1mtafXRI@q8Nr#%9m2B_@7U5bi38aNmN2sq**D5)D*+-87#P4p!bsX#$VLsnn7E52271Qi4W1%(%a1W*wzDdgDG+Sw2*^30qfSL1QP*pgWfS6-bJh3i_+zSn8!mA6Fv2pQ)9N@6x9vpzuc-N2y^%(3IV51S&#yI_NF6@u2lc0#7IuH64IqcCH&j71%_gGcdgz!RW6DTMaBduB=)d+x7NaN=e#KHlWD)Ie)a=b8FET@R+XeshW@BQoseewbo^+qZ!4F)#(5$Z(XTbWk9q^*ZjDO52(t5CKZ6&*y#NjMXokv*Hl&=aENaX1_iQFnpj2+y-74a@jP%T=Q*t^s!_2Na9dU&E0$Z$(F-FAx61vPjHwZLpIMomE@)t5D1F$^^KqPI_l+tU4u8DXuQ%(UMITqSXojNwv*J#teel^3(Nv-9w64y8(7ZV2IO@Llfw3nXGwicUFV69&C5zQ%mfv0z-%05*o(n!^*-0jP@=mXh=2V4fh2JNSHf#0F9$ySEnHfzcmIsor+K4Zw7Cngu15w59pQ_5s04p4AVNU4sc(&bm5z*t)jSU)-9(UeH_=-H(yPbRyyGhBHytC_-^^(X7$QL#^dHS(O4EimpNKb48FnzdJ=_9KUefkee+-bQ)ME7jN^@*9RGAU7iblFc-$n1j1)!4WE^^EEoVr7wMIQ=nJJwHTJeNB";
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // [--------- GetAllKeysAsync ---------]
        var allKeys = await _tdbService.GetAllKeysAsync(cancellationToken);

        // [------------ SaveAsync ------------]
        var saveResult = await _tdbService.SaveAsync(new TDBData<MyTestModel>()
        {
            Key = "item-key",
            Value = new MyTestModel()
            {
                Name = "FirstTest",
                Description = "FirstDescription",
                Type = "FuncTest"
            }
        }, cancellationToken);

        // [--------- GetAsync ---------]
        var (isSuccess, isFailed, data) = await _tdbService.GetAsync<MyTestModel>("item-key", cancellationToken);
        var itemData = isSuccess ? data : null;

        await Task.Delay(1000, cancellationToken);

        // [--------- UpdateAsync ---------]
        var updateResult = await _tdbService.UpdateAsync("item-key", new TDBData<MyTestModel>()
        {
            Key = "item-key",
            Value = new MyTestModel()
            {
                Name = "FirstTest2",
                Description = "FirstDescription2",
                Type = "FuncTest2"
            }
        }, cancellationToken);

        // [--------- DeleteAsync ---------]
        var deleteResult = await _tdbService.DeleteAsync("item-key", cancellationToken);

        await Task.Delay(5000, cancellationToken);
        await Task.Delay(1000000, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}