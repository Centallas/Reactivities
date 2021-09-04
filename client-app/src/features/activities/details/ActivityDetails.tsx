import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { Grid } from "semantic-ui-react";
import LoadingComponents from "../../../app/layout/LoadingComponent";
import { useStore } from "../../../app/stores/store";
import ActivityDetailedChat from "./ActivityDetailedChat";
import ActivityDetailedInfo from "./ActivityDetailedInfo";
import ActivityDetailedSideBar from "./ActivityDetailedSidebar";
import ActivityDetailedHeader from "./ActivityDetaledHeader";


export default observer(function ActivityDetails() {
    const { activityStore } = useStore();
    const { selectedActivity: activity, loadActivity, loadingInitial } = activityStore;
    const { id } = useParams<{ id: string }>();

    useEffect(() => {
        if (id) loadActivity(id);
    }, [id, loadActivity]);


    if (loadingInitial || !activity) return <LoadingComponents />;

    return (
        //Here always returns JSX
        <Grid>
            <Grid.Column width={10}>
                <ActivityDetailedHeader activity = {activity} />
                <ActivityDetailedInfo activity = {activity} />
                <ActivityDetailedChat />
            </Grid.Column>
            <Grid.Column width={6}>
                <ActivityDetailedSideBar activity={activity}/>
            </Grid.Column>
        </Grid>

    )
})
