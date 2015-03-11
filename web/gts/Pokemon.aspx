<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="Pokemon.aspx.cs" Inherits="PkmnFoundations.Web.gts.Pokemon" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:RequireCss Key="form" CssUrl="~/css/form.css" runat="server" />
    <pf:RequireCss Key="pkmnstats" CssUrl="~/css/pkmnstats.css" After="form" runat="server" />
    <pf:RequireCss Key="types" CssUrl="~/css/types.css" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
    <asp:PlaceHolder ID="phSummary" runat="server">
    <div class="gtsBox gtsPokemonSummary">
        <div class="row basicInfo">
            <div class="col colBasic1">
                <ul>
                <li class="nickname">
                    <asp:Literal ID="litNickname" runat="server" />
                </li>
                <li class="portrait">
                <asp:Image ID="imgPokemon" CssClass="sprite species"
                    Width="96" Height="96" runat="server" />
                </li>
                <li class="specialFlags">
                    <asp:PlaceHolder ID="phShiny" runat="server">
                        <span class="shiny">⁂</span>
                    </asp:PlaceHolder>
                    <%-- todo: use images for pkrs status --%>
                    <asp:PlaceHolder ID="phPkrs" runat="server">
                        <span class="pkrs">PKRS</span>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phPkrsCured" runat="server">
                        <span class="pkrs_cure">CURED</span>
                    </asp:PlaceHolder>
                    &nbsp;
                </li>
                <li class="marks">
                    <asp:Literal ID="litMarks" runat="server" />
                </li>
                <li>
                    <asp:Image ID="imgPokeball" CssClass="sprite item"
                        Width="24" Height="24" runat="server" />
                    Lv. 
                    <asp:Literal ID="litLevel" runat="server" />
                    <asp:Literal ID="litGender" runat="server" />
                </li>
                </ul>
                <p>Met December 19, 2014<br />
                    in Slateport City at Lv. 20</p>
                <p>Capable of taking hits.</p>
            </div>

            <div class="col colBasic2">
                <table class="summaryItems">
                    <tr>
                        <th>Species</th>
                        <td>Pikachu</td>
                    </tr>
                    <tr>
                        <th>Pokédex</th>
                        <td>025</td>
                    </tr>
                    <tr>
                        <th>Type</th>
                        <td><span class="type electric">Electric</span></td>
                    </tr>
                    <tr>
                        <th>OT</th>
                        <td>Ash</td>
                    </tr>
                    <tr>
                        <th>ID No.</th>
                        <td>12345</td>
                    </tr>
                    <tr>
                        <th>Exp.</th>
                        <td>8000

                            <div class="nextIn">next in 1261</div>
                            <div class="gtsProgress expBar">
                                <div class="progress" style="width: 50%;"></div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>Held item</th>
                        <td><asp:Image ID="imgItem" CssClass="sprite item" ImageUrl="~/images/item-sm/3202.png" Width="24" Height="24" runat="server" />
                            Light Ball</td>
                    </tr>
                    <tr>
                        <th>Nature</th>
                        <td>Naive</td>
                    </tr>
                    <tr>
                        <th>Ability</th>
                        <td>Lightning Rod<sup>dw</sup>
                        </td>
                    </tr>
                </table>
            </div>
        
            <div class="col colStats">
                <table class="summaryItems">
                    <tr>
                        <th>HP</th>
                        <td>50 / 50<br />
                            <div class="gtsProgress hpBar">
                                <div class="progress" style="width: 90%;"></div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>Attack</th>
                        <td>27</td>
                    </tr>
                    <tr>
                        <th>Defense</th>
                        <td>27</td>
                    </tr>
                    <tr>
                        <th>Sp. Atk</th>
                        <td>31</td>
                    </tr>
                    <tr>
                        <th>Sp. Def</th>
                        <td>23</td>
                    </tr>
                    <tr>
                        <th>Speed</th>
                        <td>50</td>
                    </tr>
                </table>

                <table class="moves">
                    <tr class="move normal">
                        <th class="type normal">Normal</th>
                        <td>
                            <span class="name">Quick Attack</span>
                            <span class="pp">30 / 30</span>
                        </td>
                    </tr>
                    <tr class="move electric">
                        <th class="type electric">Electric</th>
                        <td>
                            <span class="name">Electro Ball</span>
                            <span class="pp">10 / 10</span>
                        </td>
                    </tr>
                    <tr class="move electric">
                        <th class="type electric">Electric</th>
                        <td>
                            <span class="name">Thunder Wave</span>
                            <span class="pp">20 / 20</span>
                        </td>
                    </tr>
                    <tr class="move fairy">
                        <th class="type fairy">Fairy</th>
                        <td>
                            <span class="name">Drainging Kiss</span>
                            <span class="pp">10 / 10</span>
                        </td>
                    </tr>
                </table>

            </div>

        </div>
        <div class="clear"></div>

    </div>
    </asp:PlaceHolder>
</asp:Content>
