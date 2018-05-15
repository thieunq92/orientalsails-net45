﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContractCreate.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.ContractCreate"
    MasterPageFile="MO-NoScriptManager.Master" Title="Create Contract" %>

<asp:Content ID="AdminContent" ContentPlaceHolderID="AdminContent" runat="server">
    <h3 class="page-header">Create Contract</h3>
    <div class="contract-panel" ng-controller="contractCreateController">
        <div class="form-group">
            <div class="row">
                <div class="col-xs-1">
                    Name
                </div>
                <div class="col-xs-2">
                    <input type="text" class="form-control" ng-model="name" placeholder="Name"/>
                </div>
                <div class="col-xs-1">
                    Currency
                </div>
                <div class="col-xs-2">
                    <select id="currency" class="form-control" ng-model="currency" convert-to-number>
                        <option value="1">USD</option>
                        <option value="2">VND</option>
                    </select>
                </div>
            </div>
        </div>
        <div ng-controller="addRemoveValidTimeController">
            <div class="form-group" ng-repeat="item in rs.listValidTime">
                <div class="row">
                    <div class="col-xs-1">
                        Valid from
                    </div>
                    <div class="col-xs-2">
                        <input type="text" class="form-control" placeholder="Valid from (dd/mm/yyyy)" id="txtValidFrom" data-control="datetimepicker" ng-model="item.validFrom" />
                    </div>
                    <div class="col-xs-1">
                        Valid to
                    </div>
                    <div class="col-xs-2">
                        <input type="text" class="form-control" placeholder="Valid to (dd/mm/yyyy)" id="txtValidTo" data-control="datetimepicker" ng-model="item.validTo" />
                    </div>
                    <div class="col-xs-1">
                        <button type="button" class="btn btn-primary" ng-click="removeValidTime($index)" ng-show="rs.listValidTime.length > 1">Remove</button>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-12">
                        <button type="button" class="btn btn-primary" ng-click="addValidTime()">Add valid time</button>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div>
            <ul class="nav nav-tabs" role="tablist">
                <li role="presentation" class="active"><a href="#2d1n" role="tab" data-toggle="tab">2 NGÀY / 1 ĐÊM</a></li>
                <li role="presentation"><a href="#3d2n" role="tab" data-toggle="tab">3 NGÀY / 2 ĐÊM</a></li>
            </ul>
            <div class="tab-content">
                <div role="tabpanel" class="tab-pane active" id="2d1n">
                    <h4>DELUXE ORIENTAL SAILS (18 cabins & 8 cabins)</h4>
                    <div class="row" ng-controller="os2d1nController">
                        <div class="col-xs-12">
                            <table class="table table-bordered table-hover" style="text-align: center; margin-bottom: 0">
                                <tr>
                                    <td rowspan="2" style="vertical-align: middle"><strong>LOẠI PHÒNG</strong></td>
                                    <td colspan="3" ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <strong>Phòng đôi</strong>
                                    </td>
                                    <td>
                                        <strong>Phòng đơn</strong>
                                    </td>
                                    <td ng-repeat-end>
                                        <strong>Trẻ em 6 - 11 tuổi</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Deluxe
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="item.txtOs2d1nDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="item.txtOs2d1nSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="item.txtOs2d1nChildren6to11" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table class="table table-bordered table-hover" style="text-align: center; border-top: none">
                                <tr>
                                    <td colspan="" style="border-top: none" id="os2d1nCharterHeader"><strong>THUÊ TRỌN TÀU</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td colspan="4" ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td>Orientalsails Charter 18 cabins</td>
                                    <td colspan="4" ng-repeat="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs12d1nCharter" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td rowspan="2">Orientalsails Charter 8 cabins</td>
                                    <td ng-repeat-start="item in rs.listValidTime"><strong>1-4 khách </strong></td>
                                    <td><strong>5-8 khách </strong></td>
                                    <td><strong>9-12 khách </strong></td>
                                    <td ng-repeat-end><strong>13-17 khách </strong></td>
                                </tr>
                                <tr>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs22d1nCharter1to4passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs22d1nCharter5to8passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs22d1nCharter9to12passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs22d1nCharter13to17passenger" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <h4>DELUXE CALYPSO CRUISER (12 cabins)</h4>
                    <div class="row" ng-controller="cls2d1nController">
                        <div class="col-xs-12">
                            <table class="table table-bordered table-hover" style="text-align: center; margin-bottom: 0">
                                <tr>
                                    <td rowspan="2" style="vertical-align: middle"><strong>LOẠI PHÒNG</strong></td>
                                    <td colspan="3" ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <strong>Phòng đôi</strong>
                                    </td>
                                    <td>
                                        <strong>Phòng đơn</strong>
                                    </td>
                                    <td ng-repeat-end>
                                        <strong>Trẻ em 6 - 11 tuổi</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Deluxe
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtCls2d1nDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtCls2d1nSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtCls2d1nChildren6to11" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table class="table table-bordered table-hover" style="text-align: center; border-top: none">
                                <tr>
                                    <td colspan="5" style="border-top: none" id="cls2d1nCharterHeader"><strong>THUÊ TRỌN TÀU</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td>Calypso Cruise Charter</td>
                                    <td ng-repeat="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtCls2d1nCharter" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <h4>LUXURY STARLIGHT CRUISE (32 Cabins)</h4>
                    <div class="row" ng-controller="stl2d1nController">
                        <div class="col-xs-12">
                            <table class="table table-bordered table-hover" style="text-align: center; margin-bottom: 0">
                                <tr>
                                    <td rowspan="2" style="vertical-align: middle"><strong>LOẠI PHÒNG</strong></td>
                                    <td colspan="3" ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <strong>Phòng đôi</strong>
                                    </td>
                                    <td>
                                        <strong>Phòng đơn</strong>
                                    </td>
                                    <td ng-repeat-end>
                                        <strong>Giường phụ/đệm</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Deluxe
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nDeluxeDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nDeluxeSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nDeluxeExtrabed" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Executive
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nExecutiveDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nExecutiveSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nExecutiveExtrabed" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Suite
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nSuiteDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nSuiteSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nSuiteExtrabed" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table class="table table-bordered table-hover" style="text-align: center; border-top: none">
                                <tr>
                                    <td colspan="5" style="border-top: none"><strong>THUÊ TRỌN TÀU</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td><strong>1-40 khách</strong></td>
                                    <td><strong>41-50 khách</strong></td>
                                    <td><strong>51-63 khách</strong></td>
                                </tr>
                                <tr ng-repeat="item in rs.listValidTime">
                                    <td><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nCharter1to40passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nCharter41to50passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl2d1nCharter51to63passenger" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
                <div role="tabpanel" class="tab-pane active" id="3d2n">
                    <h4>DELUXE ORIENTAL SAILS (18 cabins & 8 cabins)</h4>
                    <div class="row" ng-controller="os3d2nController">
                        <div class="col-xs-12">
                            <table class="table table-bordered table-hover" style="text-align: center; margin-bottom: 0">
                                <tr>
                                    <td rowspan="2" style="vertical-align: middle"><strong>LOẠI PHÒNG</strong></td>
                                    <td colspan="3" ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <strong>Phòng đôi</strong>
                                    </td>
                                    <td>
                                        <strong>Phòng đơn</strong>
                                    </td>
                                    <td ng-repeat-end>
                                        <strong>Trẻ em 6 - 11 tuổi</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Deluxe
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs3d2nDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs3d2nSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs3d2nChildren6to11" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table class="table table-bordered table-hover" style="text-align: center; border-top: none">
                                <tr>
                                    <td colspan="" style="border-top: none" id="os3d2nCharterHeader"><strong>THUÊ TRỌN TÀU</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td colspan="4" ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td>Orientalsails Charter 18 cabins</td>
                                    <td colspan="4" ng-repeat="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs13d2nCharter" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td rowspan="2">Orientalsails Charter 8 cabins</td>
                                    <td ng-repeat-start="item in rs.listValidTime"><strong>1-4 khách </strong></td>
                                    <td><strong>5-8 khách </strong></td>
                                    <td><strong>9-12 khách </strong></td>
                                    <td ng-repeat-end><strong>13-17 khách </strong></td>
                                </tr>
                                <tr>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs23d2nCharter1to4passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs23d2nCharter5to8passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs23d2nCharter9to12passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtOs23d2nCharter13to17passenger" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <h4>DELUXE CALYPSO CRUISER (12 cabins)</h4>
                    <div class="row" ng-controller="cls3d2nController">
                        <div class="col-xs-12">
                            <table class="table table-bordered table-hover" style="text-align: center; margin-bottom: 0">
                                <tr>
                                    <td rowspan="2" style="vertical-align: middle"><strong>LOẠI PHÒNG</strong></td>
                                    <td colspan="3" ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <strong>Phòng đôi</strong>
                                    </td>
                                    <td>
                                        <strong>Phòng đơn</strong>
                                    </td>
                                    <td ng-repeat-end>
                                        <strong>Trẻ em 6 - 11 tuổi</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Deluxe
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtCls3d2nDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtCls3d2nSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtCls3d2nChildren6to11" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table class="table table-bordered table-hover" style="text-align: center; border-top: none">
                                <tr>
                                    <td colspan="5" style="border-top: none" id="cls3d2nCharterHeader"><strong>THUÊ TRỌN TÀU</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td>Calypso Cruise Charter</td>
                                    <td ng-repeat="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtCls3d2nCharter" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <h4>LUXURY STARLIGHT CRUISE (32 Cabins)</h4>
                    <div class="row" ng-controller="stl3d2nController">
                        <div class="col-xs-12">
                            <table class="table table-bordered table-hover" style="text-align: center; margin-bottom: 0">
                                <tr>
                                    <td rowspan="2" style="vertical-align: middle"><strong>LOẠI PHÒNG</strong></td>
                                    <td colspan="3" ng-repeat="item in rs.listValidTime"><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                </tr>
                                <tr>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <strong>Phòng đôi</strong>
                                    </td>
                                    <td>
                                        <strong>Phòng đơn</strong>
                                    </td>
                                    <td ng-repeat-end>
                                        <strong>Giường phụ/đệm</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Deluxe
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nDeluxeDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nDeluxeSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nDeluxeExtrabed" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Executive
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nExecutiveDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nExecutiveSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nExecutiveExtrabed" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Suite
                                    </td>
                                    <td ng-repeat-start="item in rs.listValidTime">
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nSuiteDouble" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nSuiteSingle" class="form-control" />
                                        </div>
                                    </td>
                                    <td ng-repeat-end>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nSuiteExtrabed" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table class="table table-bordered table-hover" style="text-align: center; border-top: none">
                                <tr>
                                    <td colspan="5" style="border-top: none"><strong>THUÊ TRỌN TÀU</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td><strong>1-40 khách</strong></td>
                                    <td><strong>41-50 khách</strong></td>
                                    <td><strong>51-63 khách</strong></td>
                                </tr>
                                <tr ng-repeat="item in rs.listValidTime">
                                    <td><strong>{{item.validFrom}} - {{item.validTo}}</strong></td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nCharter1to40passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nCharter41to50passenger" class="form-control" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="input-group">
                                            <span class="input-group-addon">USD</span>
                                            <input type="text" ng-model="txtStl3d2nCharter51to63passenger" class="form-control" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12">
                    <button type="button" class="btn btn-primary" ng-click="createContract()">Create contract</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript" src="/modules/sails/admin/contractcreatecontroller.js"></script>
    <script>
        $(function () {
            $("[data-control='datetimepicker']").datetimepicker({
                timepicker: false,
                format: 'd/m/Y',
                scrollInput: false,
                scrollMonth: false,
            })
        })
    </script>
    <script>
        $("#currency").change(function () {
            $(".input-group-addon").html($(this).children("option:selected").html());
        })
    </script>
</asp:Content>