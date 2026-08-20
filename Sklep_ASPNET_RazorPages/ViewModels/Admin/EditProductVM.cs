using MyApi.ViewModels;

namespace MyApi.ViewModels.Admin;
public class EditProductVM : BaseViewModel
{
    public string p_name = "";
    public string p_imgurl = "";
    public string p_description = "";
    public double p_price;
    public int p_id;
    public bool Editing = true;
}