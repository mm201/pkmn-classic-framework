<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="Pokemon.aspx.cs" Inherits="PkmnFoundations.Web.gts.Pokemon" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>
<%@ Import Namespace="PkmnFoundations.Pokedex" %>
<%@ Import Namespace="PkmnFoundations.Structures" %>

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
                <p><asp:Literal ID="litTrainerMemo" Text="Trainer memo todo" runat="server" /></p>
                <p><asp:Literal ID="litCharacteristic" Text="Characteristic todo" runat="server" /></p>
            </div>

            <div class="col colBasic2">
                <table class="summaryItems">
                    <tr>
                        <th>Species</th>
                        <td><asp:Literal ID="litSpecies" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Pokédex</th>
                        <td>#<asp:Literal ID="litPokedex" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Type</th>
                        <td>
                            <asp:Literal ID="litType1" runat="server" />&nbsp;
                            <asp:Literal ID="litType2" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>OT</th>
                        <td><asp:Literal ID="litOtName" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>ID No.</th>
                        <td><asp:Literal ID="litTrainerId" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Exp.</th>
                        <td><asp:Literal ID="litExperience" runat="server" />

                            <div class="nextIn">next in ??? todo</div>
                            <div class="gtsProgress expBar">
                                <asp:Literal ID="litExpProgress" runat="server">
                                    <div class="progress" style="width: 50%;"></div>
                                </asp:Literal>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>Held item</th>
                        <td><asp:Image ID="imgHeldItem" CssClass="sprite item" ImageUrl="~/images/item-sm/3202.png" Width="24" Height="24" runat="server" />
                            <asp:Literal ID="litHeldItem" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Nature</th>
                        <td><asp:Literal ID="litNature" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Ability</th>
                        <td><asp:Literal ID="litAbility" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        
            <div class="col colStats">
                <table class="summaryItems">
                    <tr>
                        <th>HP</th>
                        <td><asp:Literal ID="litHpCurr" runat="server" /> / <asp:Literal ID="litHp" runat="server" /><br />
                            <div class="gtsProgress hpBar">
                                <asp:Literal ID="litHpProgress" runat="server">
                                    <div class="progress" style="width: 50%;"></div>
                                </asp:Literal>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>Attack</th>
                        <td><asp:Literal ID="litAtk" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Defense</th>
                        <td><asp:Literal ID="litDef" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Sp. Atk</th>
                        <td><asp:Literal ID="litSAtk" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Sp. Def</th>
                        <td><asp:Literal ID="litSDef" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Speed</th>
                        <td><asp:Literal ID="litSpeed" runat="server" /></td>
                    </tr>
                </table>

                <table class="moves">
                    <asp:Repeater ID="rptMoves" runat="server">
                        <ItemTemplate>
                            <tr class="move <%# ((MoveSlot)Container.DataItem).Move == null ? "" : ((MoveSlot)Container.DataItem).Move.Type.Identifier %>">
                                <th class="type <%# ((MoveSlot)Container.DataItem).Move == null ? "" : ((MoveSlot)Container.DataItem).Move.Type.Identifier %>">
                                    <%# ((MoveSlot)Container.DataItem).Move == null ? "" : ((MoveSlot)Container.DataItem).Move.Type.Name.ToString() %>
                                </th>
                                <td>
                                    <span class="name">
                                    <%# ((MoveSlot)Container.DataItem).Move == null ? "&nbsp;" : ((MoveSlot)Container.DataItem).Move.Name.ToString() %>
                                    </span>
                                    <span class="pp">
                                    <%# ((MoveSlot)Container.DataItem).Move == null ? "" : ((MoveSlot)Container.DataItem).RemainingPP.ToString() + " / " %>
                                    <%# ((MoveSlot)Container.DataItem).Move == null ? "" : ((MoveSlot)Container.DataItem).PP.ToString() %>
                                    </span>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>

            </div>

            <div class="col colRibbons">

                <asp:Repeater ID="rptRibbons" runat="server">

                    <ItemTemplate>
                        <asp:Image ImageUrl='<%# "~/images/ribbon-sm/" + ((Ribbon)Container.DataItem).ID.ToString() + ".png" %>'
                            ToolTip='<%# ((Ribbon)Container.DataItem).Name.ToString() %>' CssClass="sprite" runat="server" />
                    </ItemTemplate>

                </asp:Repeater>

                <asp:Repeater ID="rptUnknownRibbons" runat="server">

                    <ItemTemplate>
                        Unknown ribbon in position <%# ((int)Container.DataItem).ToString() %>.
                    </ItemTemplate>

                </asp:Repeater>

            </div>

        </div>
        <div class="clear"></div>

    </div>
    </asp:PlaceHolder>
</asp:Content>
