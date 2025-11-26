using Autodesk.ProductInterface.PowerMILL;
using MoldAutomation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerMill_API_Version1._0.Macros
{
    [Obsolete]
    public class FaceMill
    {
        private readonly PMAutomation _powerMill;
        public  FaceMill(PMAutomation powerMill)
        {
            _powerMill = powerMill;
        }

        public void Run()
        {

            _powerMill.Execute("FORM STRATEGYSELECTOR");
            _powerMill.Execute("STRATEGYSELECTOR CATEGORY 'Curve-Machining' NEW");
            _powerMill.Execute("STRATEGYSELECTOR STRATEGY \"Curve-Machining/Face-Milling.002.ptf\" NEW");

            _powerMill.Execute("IMPORT TEMPLATE ENTITY TOOLPATH TMPLTSELECTORGUI \"Curve-Machining/Face-Milling.002.ptf\"");

            // let the user input parameters inside PowerMill
            _powerMill.Execute("REAL $VarxyExpansion = INPUT \" Input XY Expansion\"");
            _powerMill.Execute("REAL $VarStepover = INPUT \" Input Stepover\"");
            _powerMill.Execute("REAL $VarStockFace = INPUT \" Stock  on face\"");
            _powerMill.Execute("REAL $VarStepdown = INPUT \" Enter Stepdown\"");
            _powerMill.Execute("REAL $VarRasterAngle = INPUT \" Raster Angle\"");

            _powerMill.Execute("EDIT TPPAGE SWBlock");
            _powerMill.Execute("EDIT BLOCK RESET");
            _powerMill.Execute("EDIT BLOCK XLEN LOCK");
            _powerMill.Execute("EDIT BLOCK YLEN LOCK");
            _powerMill.Execute("EDIT BLOCK ZMIN LOCK");
            _powerMill.Execute("EDIT BLOCK ZMAX \".1\"");
            _powerMill.Execute("EDIT BLOCK RESET");
            _powerMill.Execute("EDIT BLOCK RESETLIMIT $VarStockFace");
            _powerMill.Execute("EDIT BLOCK RESET");
            _powerMill.Execute("EDIT TPPAGE TOOL");
            _powerMill.Execute("EDIT TPPAGE SWMachineTool");
            _powerMill.Execute("EDIT TPPAGE TOOL");
            _powerMill.Execute("ACTIVATE TOOL \"CUTTER D 25\"");
            _powerMill.Execute("RENAME TOOLPATH # \"Facing\"");
            _powerMill.Execute("EDIT TPPAGE SWCutDistVert");
            _powerMill.Execute("EDIT TPPAGE SWFaceMilling");
            _powerMill.Execute("Edit par 'Face.Z' \"0\"");
            _powerMill.Execute("Edit par 'Face.XYExpansion' $VarxyExpansion");
            _powerMill.Execute("Edit par 'PatternStyle' 'two_way'");
            _powerMill.Execute("Edit par 'RadialDepthOfCut.UserDefined' '1' Edit par 'Stepover' $VarStepover");
            _powerMill.Execute("EDIT TPPAGE SWCutDistVert");
            _powerMill.Execute("Edit par 'CutDistances.Axial.StockDepth' $VarStockFace");
            _powerMill.Execute("Edit par 'AxialDepthOfCut.UserDefined' $VarStockFace Edit par 'Stepdown' $VarStockFace");
            _powerMill.Execute("Edit par 'AxialDepthOfCut.UserDefined' '1' Edit par 'Stepdown' \"$VarStepdown\"");
            _powerMill.Execute("EDIT TPPAGE SWFinishFlrWall");
            _powerMill.Execute("EDIT TPPAGE SWFaceRaster");
            _powerMill.Execute("EDIT PAR 'AutomaticRasterAngle' '0'");
            _powerMill.Execute("Edit par 'RasterAngle' \"$VarRasterAngle\"");
            _powerMill.Execute("EDIT TPPAGE SWAutoVerifMdlG");
            _powerMill.Execute("EDIT TPPAGE SWToolAxOVec");
            _powerMill.Execute("EDIT TPPAGE SWToolRapidMvClear");
            _powerMill.Execute("EDIT TPPAGE SWLeadIn");
            _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN EXTENDED");
            _powerMill.Execute("EDIT TOOLPATH LEADS LEADIN DISTANCE \"10\"");
            _powerMill.Execute("EDIT TOOLPATH LEADS LEADOUT COPY");
            _powerMill.Execute("EDIT TPPAGE SWLink");
            _powerMill.Execute("EDIT TOOLPATH \"Facing\" CALCULATE");
            _powerMill.Execute("EDIT TPPAGE SWFeedSpeed");
            _powerMill.Execute("EDIT TOOLPATH \"Facing\" RECYCLE");
            _powerMill.Execute("EDIT RPM \"3000\"");
            _powerMill.Execute("EDIT FRATE \"2800\"");
            _powerMill.Execute("EDIT PRATE \"2800\"");
            _powerMill.Execute("EDIT RSPEED \"8000\"");
            _powerMill.Execute("EDIT TOOLPATH \"Facing\" CALCULATE");
            _powerMill.Execute("FORM ACCEPT SFFaceMilling");
            _powerMill.Execute("EDIT BLOCK RESET");
            _powerMill.Execute("EDIT BLOCK XLEN UNLOCK");
            _powerMill.Execute("EDIT BLOCK YLEN UNLOCK");
            _powerMill.Execute("EDIT BLOCK ZLEN UNLOCK");



        }   
    }
}
