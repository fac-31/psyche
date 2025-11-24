# Content Generation Plan
## 1. Task
### 1.1. Overview
For each `<Storylet>` item in the `xml` below, one `jsonc` file should be created in `Data/Storylets`

### 1.2. Rules

#### 1.2.1. Storylets
Each created storylet must adhere to the following...

- 1.2.1.1. All storylets must be in the exact structure required by `../../../../Data/Json/StoryletDto.cs`
- 1.2.1.2. Requirements listed in the XML entry must be strictly adhered to
- 1.2.1.3. All storylets must have exactly 3 selectable options
- 1.2.1.4. Where `<Requirements>` are not defined, create them (avoiding extreme values)
- 1.2.1.5. Where `<Effects>` are not defined, create them (avoiding extreme values)

#### 1.2.2. Options
- 1.2.2.1. *Exactly one* Option per `<Storylet>` must have no prerequisites
- 1.2.2.2. *Every other* Option must have Prerequisites
- 1.2.2.3. *Every* Option prerequisite must *only* test conditions ignored by the parent `<Storylet>`
- 1.2.2.4. All Options must apply Effects
- 1.2.2.5. All flavour text must be coherent with the mechanical effects

### 1.3. Narrative Guidance

- 1.3.1. Derive your understanding of the game's world and tone from the names of the attributes, statistics and qualities defined within the code

---

## 2. XML

```xml
<Storylets>
  <Storylet><Requirement type="Attribute" check="SelfAssurance above 70"/></Storylet>
  <Storylet><Requirement type="Attribute" check="SelfAssurance below 30"/></Storylet>
  <Storylet><Requirement type="Attribute" check="SelfAssurance between 30 and 70"/></Storylet>
  <Storylet><Effect type="Attribute" impact="SelfAssurance gain"/></Storylet>
  <Storylet><Effect type="Attribute" impact="SelfAssurance loss"/></Storylet>
  
  <Storylet><Requirement type="Attribute" check="Compassion above 70"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Compassion below 30"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Compassion between 30 and 70"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Compassion gain"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Compassion loss"/></Storylet>
  
  <Storylet><Requirement type="Attribute" check="Ambition above 70"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Ambition below 30"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Ambition between 30 and 70"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Ambition gain"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Ambition loss"/></Storylet>
  
  <Storylet><Requirement type="Attribute" check="Drive above 70"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Drive below 30"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Drive between 30 and 70"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Drive gain"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Drive loss"/></Storylet>
  
  <Storylet><Requirement type="Attribute" check="Discernment above 70"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Discernment below 30"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Discernment between 30 and 70"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Discernment gain"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Discernment loss"/></Storylet>
  
  <Storylet><Requirement type="Attribute" check="Bravery above 70"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Bravery below 30"/></Storylet>
  <Storylet><Requirement type="Attribute" check="Bravery between 30 and 70"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Bravery gain"/></Storylet>
  <Storylet><Effect type="Attribute" impact="Bravery loss"/></Storylet>
  
  <Storylet><Requirement type="Quality" check="low secrets_learned"/></Storylet>
  <Storylet><Requirement type="Quality" check="high secrets_learned"/></Storylet>
  <Storylet><Effect type="Quality" impact="secrets_learned loss"/></Storylet>
  <Storylet><Effect type="Quality" impact="secrets_learned gain"/></Storylet>
  
  <Storylet><Requirement type="Quality" check="low psychological_strain"/></Storylet>
  <Storylet id="A1"><Requirement type="Quality" check="high psychological_strain"/></Storylet>
  <Storylet><Effect type="Quality" impact="psychological_strain loss"/></Storylet>
  <Storylet><Effect type="Quality" impact="psychological_strain gain"/></Storylet>
  <Storylet id="A2"><Requirement type="StoryletPlayed" check="A1"/></Storylet>
  <Storylet id="A3"><Requirement type="StoryletPlayed" check="A2"/></Storylet>
  <Storylet id="A4"><Requirement type="StoryletPlayed" check="A3"/></Storylet>
  <Storylet id="A5"><Requirement type="StoryletPlayed" check="A4"/></Storylet>
  
  <Storylet><Requirement type="Quality" check="low social_capital"/></Storylet>
  <Storylet id="B1"><Requirement type="Quality" check="high social_capital"/></Storylet>
  <Storylet><Effect type="Quality" impact="social_capital loss"/></Storylet>
  <Storylet><Effect type="Quality" impact="social_capital gain"/></Storylet>
  <Storylet id="A6"><Requirement type="StoryletPlayed" check="A5"/></Storylet>
  <Storylet id="B2"><Requirement type="StoryletPlayed" check="B1"/></Storylet>
  <Storylet id="B3"><Requirement type="StoryletPlayed" check="B2"/></Storylet>
  <Storylet id="B4"><Requirement type="StoryletPlayed" check="B3"/></Storylet>
  <Storylet id="B5"><Requirement type="StoryletPlayed" check="B4"/></Storylet>
  <Storylet id="B6"><Requirement type="StoryletPlayed" check="B5"/></Storylet>
  
  <Storylet><Requirement type="Quality" check="low enemies_made"/></Storylet>
  <Storylet id="C1"><Requirement type="Quality" check="high enemies_made"/></Storylet>
  <Storylet><Effect type="Quality" impact="enemies_made loss"/></Storylet>
  <Storylet><Effect type="Quality" impact="enemies_made gain"/></Storylet>
  <Storylet id="C2"><Requirement type="StoryletPlayed" check="C1"/></Storylet>
  <Storylet id="C3"><Requirement type="StoryletPlayed" check="C2"/></Storylet>
  <Storylet id="C4"><Requirement type="StoryletPlayed" check="C3"/></Storylet>
  <Storylet id="C5"><Requirement type="StoryletPlayed" check="C4"/></Storylet>
  <Storylet id="C6"><Requirement type="StoryletPlayed" check="C5"/></Storylet>
</Storylets>
```