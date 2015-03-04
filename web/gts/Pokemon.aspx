<%@ Page Title="" Language="C#" MasterPageFile="~/masters/MasterPage.master" AutoEventWireup="true" CodeBehind="Pokemon.aspx.cs" Inherits="PkmnFoundations.Web.gts.Pokemon" %>
<%@ Register TagPrefix="pf" Namespace="PkmnFoundations.Web" Assembly="PkmnFoundations.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">
    <pf:RequireCss Key="form" CssUrl="~/css/form.css" runat="server" />
    <pf:RequireCss Key="pkmnstats" CssUrl="~/css/pkmnstats.css" After="form" runat="server" />
    <pf:RequireCss Key="types" CssUrl="~/css/types.css" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cpMain" runat="server">
    <div class="gtsBox gtsPokemonSummary">
        <div class="row basicInfo">
            <div class="col basic1">
                <ul>
                <li class="nickname">
                    SparkySparky
                </li>
                <li class="portrait">
                <asp:Image ID="imgPokemon" CssClass="sprite species" ImageUrl="~/images/pkmn-lg-s/25.png" 
                    Width="96" Height="96" AlternateText="Pikachu" runat="server" />
                </li>
                <li class="specialFlags">
                    <span class="shiny">⁂</span>
                    <%-- todo: use images for pkrs status --%>
                    <span class="pkrs">PKRS</span>
                    <span class="pkrs_cure">CURED</span>
                    &nbsp;
                </li>
                <li class="marks">
                    <span class="m">●</span>
                    <span class="m">▲</span>
                    <span>■</span>
                    <span class="m">♥</span>
                    <span class="m">★</span>
                    <span class="m">♦</span>
                </li>
                <li>
                    <asp:Image ID="imgPokeball" CssClass="sprite item" ImageUrl="~/images/item-sm/3004.png"
                        Width="24" Height="24" runat="server" />
                    Lv. 20 ♂
                </li>
                </ul>
                <p>Met December 19, 2014<br />
                    in Slateport City at Lv. 20</p>
                <p>Capable of taking hits.</p>
            </div>

            <div class="col basic2">
                <table>
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
                        <td>8000</td>
                    </tr>
                    <tr>
                        <th>Next in</th>
                        <td>1261<br />
                            <div class="pfProgressBar expBar">
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
        
            <div class="col stats">
                <table>
                    <tr>
                        <th>HP</th>
                        <td>50 / 50<br />
                            <div class="progressBar hpBar">
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
            </div>

            <div class="col moves">
                <ul>
                    <li class="move">
                        <span class="name">Quick Attack</span>
                        <span class="type normal">Normal</span>
                        <span class="pp">PP</span>
                        <span class="ppq">30 / 30</span>
                    </li>
                    <li class="move">
                        <span class="name">Electro Ball</span>
                        <span class="type electric">Electric</span>
                        <span class="pp">PP</span>
                        <span class="ppq">10 / 10</span>
                    </li>
                    <li class="move">
                        <span class="name">Thunder Wave</span>
                        <span class="type electric">Electric</span>
                        <span class="pp">PP</span>
                        <span class="ppq">20 / 20</span>
                    </li>
                    <li class="move">
                        <span class="name">Drainging Kiss</span>
                        <span class="type fairy">Fairy</span>
                        <span class="pp">PP</span>
                        <span class="ppq">10 / 10</span>
                    </li>
                </ul>
            </div>

        </div>
        <div class="clear"></div>

    </div>
</asp:Content>
