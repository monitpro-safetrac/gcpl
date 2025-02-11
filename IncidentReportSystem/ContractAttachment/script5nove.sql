USE [SAFETRACK]
GO
/****** Object:  Table [dbo].[AllWorkTypeCheckList]    Script Date: 11/5/2020 12:29:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AllWorkTypeCheckList](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[WorkTypeID] [int] NOT NULL,
	[Name] [varchar](500) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GeneralCheckListTemplate]    Script Date: 11/5/2020 12:29:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GeneralCheckListTemplate](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](300) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[AllWorkTypeCheckList] ON 

INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (1, 1, N'(HOT WORK): Equipment / Work area checked')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (2, 1, N'Firewater system checked for readiness')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (3, 1, N'Firewater hose/  Portable extinguisher provided')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (4, 1, N'Equipment Steamed /N2 purged')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (5, 1, N'Equipment blinded / Disconnected / Closed / Isolated / wedged open')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (6, 1, N'All Flammable materials removed')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (7, 1, N'Proper means of Escape & Rescue avaiable or provided')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (8, 1, N'Checked spark arrestor on mobile equipment')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (9, 1, N'"Welding machine checked, earthed & located at safe location"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (10, 1, N'Shield / booth against sparks provided')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (11, 1, N'Earthing of the equipment verified')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (12, 1, N'Oxygen / Acetylene cylinder kept outside Confined space (tank / Vessel / Column etc.)')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (13, 1, N'Considered hazard from other routine / non routine operations and concerned persons alerted')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (14, 1, N'ELCB provided on portable electrical equipment')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (15, 1, N'Flash back arrestor provided on gas cutting set / Cylinders')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (16, 1, N'"Scaffolding, ladders and Platform checked and OK"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (17, 3, N'(CONFINED SPACE): Considered hazard from other routine / non routine operations')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (18, 3, N'Concerned persons alerted')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (19, 3, N'Equipment blinded / disconnected / closed / isolated / wedged/opened')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (20, 3, N'Blind list attached (Vessel / columns / Process Equipments etc.)')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (21, 3, N'Adequate ventilation provided inside the workspace')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (22, 3, N'Adequate (24 V) lighting provided')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (23, 3, N'Exhaust Fan provided')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (24, 3, N'Safety belt/ lifeline provided ')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (25, 3, N'Provision to rescue available ')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (26, 3, N'Moving parts de-energized')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (27, 3, N'Use of Air hose / Fresh air mask / Breathing apparatus')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (28, 3, N'Means of communication provided.')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (29, 3, N'Proper means of access / exit provided')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (30, 3, N'Inert gas/LPG disconnected')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (31, 3, N'Ladder arranged in good condition')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (32, 3, N'Valve clossed/tagged/locked')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (33, 3, N'Positive Isolation of Lines/Equipment ensured by?........................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (34, 4, N'(ELECTRICAL): The apparatus to be worked upon has been isolated a) Racking out of Breaker b)Switching off of FSUs c)Removal of main fuses d)Removal of Neutral link')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (35, 4, N'Power Supply a) Isolated from supply end b) Removal of Main fuses')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (36, 4, N'Control supply a)Isolation of control supply b)Removal of control fuses')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (37, 4, N'Space heater Supply a)Isolation of control supply b)Removal of control fuses')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (38, 4, N'Interlocking or any other supply a) Isolation of control supply b) Removal of control fuses')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (39, 4, N'"In case of HT equipment, checking with discharge rod /by any other method to make sure the equipment is dead and safe to work"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (40, 4, N'"In case of cutting and removal of cable, identification of cable by suitable means to confirm the cable & it?s dead condition"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (41, 4, N'Ensuring proper tools / tackles / measuring instruments and other safety equipments')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (42, 4, N'Continuous supervision by  KPL person/ Contractor Supervisor during execution of the job')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (43, 4, N'Carrying out of Earthing by suitable means')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (44, 4, N'"In case HT Breaker, Locking of safety shutters / door"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (45, 4, N'"Checking of Isolation of apparatus visually, electrically and mechanically"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (46, 4, N'Fixing of Danger tags with Signature')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (47, 4, N'Fixing of Caution tag / Notice on it or adjacent to live apparatus')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (48, 5, N'(EXCAVATION): Details of  Excavation : Length..............m width?..............m Depth??......?..m')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (49, 5, N'Area marked and checked by Electrical/Instrument Manager')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (50, 5, N'Underground cables checked ')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (51, 5, N'Underground pipes checked')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (52, 5, N'Hand tools are insulated')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (53, 5, N'Pointed axe not used in the vicinity of cables ')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (54, 5, N'Shoring arrangement done')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (55, 5, N'Barricaded with safety sign')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (56, 5, N'Continuous supervision present during work ')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (57, 5, N'Electrical details of underground cables & precaution to be taken.................................................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (58, 5, N'Instrument details of underground cables / pipes & precaution to be taken..........................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (59, 5, N'Mechanical details of underground cables / pipes & precaution to be taken..........................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (60, 5, N'Permit excavation sketch given- Refer the last section (or) attached picture')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (61, 5, N'Power lines / HT lines are not in the close proximity of work area or isolated /disconnected. (write details)')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (62, 6, N'(LIFTING): Details of Load : Length............m; width..............m; Dia?................m; weight ?........................ Tonnes')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (63, 6, N'"Lifting equipment  and tools and tackles (websling, wire rope etc) duly tested. Test Certificate available."')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (64, 6, N'Safe working load of crane................Tonnes')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (65, 6, N'Crane/ Hydra checks done as per OEM needs ')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (66, 6, N'Valid statutory test certificates available')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (67, 6, N'Check Operator Licence  validity')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (68, 6, N'Enclose all the crane documents')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (69, 6, N'"Precautions taken for overhead cables, pipe racks"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (70, 6, N'Condition of floor stable for Crane')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (71, 6, N'Rigging area barricaded ')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (72, 6, N'Rigging supervisor available ')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (73, 6, N'Adequate lighting provided')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (74, 6, N'Power lines / HT lines are not in the close proximity of work area or isolated /disconnected. (Write details)')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (75, 6, N'Is there any overhead restrictions? Specify?.................................................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (76, 6, N'Is there any crane movement restrictions? Specify?.................................................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (77, 6, N'Is there any below ground restrictions? Specify?.................................................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (78, 6, N'Precaution taken against Road traffic restrictions?.................................................................................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (79, 6, N'Crane operator License ?.......................................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (80, 6, N'Insurance for Crane ?.............................................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (81, 6, N'Registration certificate for Crane?.................................................................................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (82, 7, N'"(WORK AT HEIGHT): Area cordoned off / covering of floor openings, & display of caution boards / tags at site."')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (83, 7, N'Precautions against falling of material in work area and material carried up at elevation are fall protected.')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (84, 7, N'Provision of roof ladder / scaffold for working at hight')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (85, 7, N'Number of persons likely to work on the platform?.....................')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (86, 7, N'Provision of safety net at site (as per nature of job)')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (87, 7, N'Provision of fall arrester at site (as per nature of job )')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (88, 7, N'"Scaffolding erected as per KPL Standard, inspected and display of safe to use board at site"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (89, 7, N'"Alternate means of escape / exit available at work site (Fixed ladder with 1 Meter projection, Platform without any obstructions)"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (90, 7, N'Provision of ELCB for use of portable tools / equipments being used at work site.')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (91, 7, N'Power lines / HT lines are not in the close proximity of work area or isolated /disconnected.')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (92, 7, N'"Workmen do not suffer from vertigo, are trained and  experienced, authorized to work at height"')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (93, 7, N'Proper ventilation provided at work site.')
INSERT [dbo].[AllWorkTypeCheckList] ([ID], [WorkTypeID], [Name]) VALUES (94, 7, N'EC Required')
SET IDENTITY_INSERT [dbo].[AllWorkTypeCheckList] OFF
SET IDENTITY_INSERT [dbo].[GeneralCheckListTemplate] ON 

INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (1, N'Equipment Isolated')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (2, N'Valve isolation')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (3, N'Double valve isolation')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (4, N'Drain/ Depressurize the equipment')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (5, N'Purge/Waterflush the equipment')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (6, N'Verify mobile equipment inspected')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (7, N'"Electrical Lockout, Tagout & Line clearance obtained"')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (8, N'"Sewer, manholes, etc. and hot surfaces nearby covered"')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (9, N'Surrounding area checked & cleaned up of oil /rags/ grass etc')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (10, N'Display of Contractors work execution board at site')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (11, N'Area cardoned off')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (12, N'Use Non sparking Tools')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (13, N'Slip plate / Blind provided')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (14, N'Proper lighting provided at work site')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (15, N'Proper ventilation provided at work site')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (16, N'Display of caution boards / tags provided')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (17, N'Safe means of access avaiable')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (18, N'Discussion of job related Hazards')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (19, N'Considered hazard from other routine / non-routine operations and concerned persons alerted')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (20, N'Standby person provided for continuous supervision (Contractor / Process / Maintenance)')
INSERT [dbo].[GeneralCheckListTemplate] ([ID], [Name]) VALUES (21, N'Gas Tests: mention values in the table')
SET IDENTITY_INSERT [dbo].[GeneralCheckListTemplate] OFF
