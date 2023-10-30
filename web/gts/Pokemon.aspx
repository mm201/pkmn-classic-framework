<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="Pokemon.aspx.cs" Inherits="PkmnFoundations.Web.gts.Pokemon" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>
<%@ Import Namespace="PkmnFoundations.Pokedex" %>
<%@ Import Namespace="PkmnFoundations.Structures" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:RequireCss Key="form" CssUrl="~/css/form.css" runat="server" />
    <pf:RequireCss Key="pkmnstats" CssUrl="~/css/pkmnstats.css" After="form" runat="server" />
    <pf:RequireCss Key="types" CssUrl="~/css/types.css" runat="server" />

    <style type="text/css">
        .gtsPokemonSummary .pfGroupStats .pfFormValue.bigstat, .gtsPokemonSummary .pfGroupStats .pfFormKey.bigstat
        {
            width: 108px;
        }

        .gtsPokemonSummary .pfGroupStats .pfFormValue.smallstat, .gtsPokemonSummary .pfGroupStats .pfFormKey.smallstat
        {
            width: 20px;
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
    <asp:PlaceHolder ID="phSummary" runat="server">
    <div class="gtsBox gtsPokemonSummary">
        <div class="row basicInfo">
            <div class="pfColumn colBasic1">
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
                        <span class="shiny">★</span>
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

            <div class="pfColumn colBasic2">
                <table class="pfFormGroup pfGroupBasic2">
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Species</th>
                        <td class="pfFormValue"><asp:Literal ID="litSpecies" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Pokédex</th>
                        <td class="pfFormValue">#<asp:Literal ID="litPokedex" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Type</th>
                        <td class="pfFormValue">
                            <asp:Literal ID="litType1" runat="server" />&nbsp;
                            <asp:Literal ID="litType2" runat="server" />
                        </td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">OT</th>
                        <td class="pfFormValue"><asp:Literal ID="litOtName" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">ID No.</th>
                        <td class="pfFormValue"><asp:Literal ID="litTrainerId" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Exp.</th>
                        <td class="pfFormValue"><asp:Literal ID="litExperience" runat="server" />

                            <div class="nextIn"><asp:Literal ID="litExperienceNext" runat="server" /></div>
                            <div class="gtsProgress expBar">
                                <asp:Literal ID="litExpProgress" runat="server">
                                    <div class="progress" style="width: 50%;"></div>
                                </asp:Literal>
                            </div>
                        </td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Held item</th>
                        <td class="pfFormValue"><asp:Image ID="imgHeldItem" CssClass="sprite item" ImageUrl="~/images/item-sm/3202.png" Width="24" Height="24" runat="server" />
                            <asp:Literal ID="litHeldItem" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Nature</th>
                        <td class="pfFormValue"><asp:Literal ID="litNature" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Ability</th>
                        <td class="pfFormValue"><asp:Literal ID="litAbility" runat="server" />
                        </td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Version</th>
                        <td class="pfFormValue"><asp:Literal ID="litVersion" runat="server" />
                        </td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Hax check</th>
                        <td class="pfFormValue"><asp:Literal ID="litHaxCheck" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        
            <div class="pfColumn colStats">
                <table class="pfFormGroup pfGroupStats">
                    <tr>
                        <th></th>
                        <th class="pfFormKey smallstat">IV</th>
                        <th class="pfFormKey smallstat">EV</th>
                        <th class="pfFormKey bigstat">Stat</th>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">HP</th>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litHpIv" runat="server" /></td>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litHpEv" runat="server" /></td>
                        <td class="pfFormValue bigstat"><asp:Literal ID="litHpCurr" runat="server" /> / <asp:Literal ID="litHp" runat="server" /><br />
                            <div class="gtsProgress hpBar">
                                <asp:Literal ID="litHpProgress" runat="server">
                                    <div class="progress" style="width: 50%;"></div>
                                </asp:Literal>
                            </div>
                        </td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Attack</th>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litAtkIv" runat="server" /></td>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litAtkEv" runat="server" /></td>
                        <td class="pfFormValue bigstat"><asp:Literal ID="litAtk" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Defense</th>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litDefIv" runat="server" /></td>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litDefEv" runat="server" /></td>
                        <td class="pfFormValue bigstat"><asp:Literal ID="litDef" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Sp. Atk</th>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litSAtkIv" runat="server" /></td>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litSAtkEv" runat="server" /></td>
                        <td class="pfFormValue bigstat"><asp:Literal ID="litSAtk" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Sp. Def</th>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litSDefIv" runat="server" /></td>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litSDefEv" runat="server" /></td>
                        <td class="pfFormValue bigstat"><asp:Literal ID="litSDef" runat="server" /></td>
                    </tr>
                    <tr class="pfFormPair">
                        <th class="pfFormKey">Speed</th>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litSpeedIv" runat="server" /></td>
                        <td class="pfFormValue smallstat"><asp:Literal ID="litSpeedEv" runat="server" /></td>
                        <td class="pfFormValue bigstat"><asp:Literal ID="litSpeed" runat="server" /></td>
                    </tr>
                </table>

                <table class="pfFormGroup pfGroupMoves">
                    <asp:Repeater ID="rptMoves" runat="server">
                        <ItemTemplate>
                            <tr class="move <%# ((MoveSlot)Container.DataItem).Move == null ? "" : ((MoveSlot)Container.DataItem).Move.Type.Identifier %>">
                                <th class="pfFormKey type <%# ((MoveSlot)Container.DataItem).Move == null ? "" : ((MoveSlot)Container.DataItem).Move.Type.Identifier %>">
                                    <%# ((MoveSlot)Container.DataItem).Move == null ? "" : ((MoveSlot)Container.DataItem).Move.Type.Name.ToString() %>
                                </th>
                                <td class="pfFormValue">
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

        <div>
        </div>

    </div>
    </asp:PlaceHolder>
</asp:Content>
